shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_disabled,unshaded,world_vertex_coords;

uniform sampler2D albedo : source_color;
uniform float bright_adjust = 1;
uniform vec4 color_adjust : source_color;
uniform sampler2D normal;
uniform float normal_strength = 1;
uniform bool use_normal;
uniform float falloff_max_distance;
uniform sampler2D wind_velocity_map;
uniform vec3 wind_velocity_map_corner;
uniform float wind_velocity_map_size; 

void vertex() {
	float bendiness = 0.04f;

	// pos -> coord
	vec3 coord = (VERTEX - wind_velocity_map_corner) / wind_velocity_map_size;
	vec2 wind = texture(wind_velocity_map, vec2(coord.x, coord.z)).rg;

	vec3 dir = normalize(vec3(wind.x, 0, wind.y));
	float mag = length(wind);

	float bend_amount = bendiness * min(mag, 10.);
	bend_amount += bend_amount * .5 * sin(TIME * 10. + VERTEX.x * 201. + VERTEX.z * 1000.);

	VERTEX += dir * (1. - UV.y) * bend_amount;
}

void fragment() {
	ALBEDO = texture(albedo, UV).rgb * bright_adjust + color_adjust.rgb;
    ALPHA = texture(albedo, UV).a;
    if (use_normal) NORMAL_MAP = texture(normal, UV).rgb * normal_strength;
    ROUGHNESS = 1.;
    SPECULAR = 0.;

	ALPHA_SCISSOR_THRESHOLD = 0.5;
	{
        // This bit converted from a standardmaterial
		float fade_distance = length(VERTEX);
		vec3 magic = vec3(0.06711056f, 0.00583715f, 52.9829189f);
        float fade = clamp(smoothstep(falloff_max_distance, sqrt(falloff_max_distance), fade_distance), 0.0, 1.0);
		if (fade < 0.001 || fade < fract(magic.z * fract(dot(FRAGCOORD.xy, magic.xy)))) {
			discard;
		}
	}

}

shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_disabled,diffuse_burley,specular_disabled;

uniform sampler2D albedo;
uniform sampler2D normal;
uniform float normal_strength = 1;
uniform bool use_normal;
uniform float falloff_max_distance;


void vertex() {
	
}

void fragment() {
	ALBEDO = texture(albedo, UV).rgb;
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

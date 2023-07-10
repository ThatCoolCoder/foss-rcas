// shader adding noise as seen through an FPV camera
// very prototypical so is pretty terrible code

shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;
uniform float time_multiplier = 2.565;
uniform float blur_size = .001;
uniform float hotspot_chance = .001;
uniform float color_quantize_amount = .05;
uniform float color_clip_max = .9;
uniform float color_clip_min = .1;
uniform float color_remap_max = .9;
uniform float color_remap_min = .1;
uniform float noise_add_multiplier = 0.1f;
uniform float noise_scale = 32.234;

float round_to_multiple(float value, float multiple)
{
	int div = int(round(value / multiple));
	return float(div) * multiple;
}

float random(vec3 p) {
    return fract(sin(dot(p.xyz,
        vec3(12.9898,78.233,328.342))) * 43758.5453123);
}

float map_number(float num, float old_min, float old_max, float new_min, float new_max)
{
	return clamp(new_min + (num - old_min) * (new_max - new_min) / (old_max - old_min), new_min, new_max);
}

void fragment()
{
	vec4 screen_color = texture(screen_texture, SCREEN_UV + vec2(blur_size, 0));
	screen_color += texture(screen_texture, SCREEN_UV + vec2(-blur_size, 0));
	screen_color += texture(screen_texture, SCREEN_UV + vec2(0, blur_size));
	screen_color += texture(screen_texture, SCREEN_UV + vec2(0, -blur_size));
	screen_color /= 4.;

	vec2 noise_uv = SCREEN_UV * noise_scale;
	noise_uv = round(noise_uv * float(100)) / float(100);
	screen_color += (random(vec3(noise_uv, -TIME * time_multiplier)) - .5) * noise_add_multiplier;

	screen_color.r = round_to_multiple(screen_color.r, color_quantize_amount);
	screen_color.g = round_to_multiple(screen_color.g, color_quantize_amount);
	screen_color.b = round_to_multiple(screen_color.b, color_quantize_amount);

	screen_color.r = map_number(map_number(screen_color.r, color_clip_min, color_clip_max, 0., 1.), color_clip_min, color_clip_max, color_remap_min, color_remap_max);
	screen_color.g = map_number(map_number(screen_color.g, color_clip_min, color_clip_max, 0., 1.), color_clip_min, color_clip_max, color_remap_min, color_remap_max);
	screen_color.b = map_number(map_number(screen_color.b, color_clip_min, color_clip_max, 0., 1.), color_clip_min, color_clip_max, color_remap_min, color_remap_max);

	screen_color.rgb += vec3((1. - color_clip_max) / 2.);

	if (random(vec3(SCREEN_UV, time_multiplier * TIME)) > 1. - hotspot_chance) screen_color.rgb = vec3(1.);
	COLOR = screen_color;
}
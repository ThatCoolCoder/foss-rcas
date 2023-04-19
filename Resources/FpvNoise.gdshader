// shader adding noise as seen through an FPV camera
// very prototypical so is pretty terrible code

shader_type canvas_item;

uniform sampler2D noise;
uniform float time_multiplier = 2.565;
uniform float blur_size = .001;
uniform float color_quantize_amount = .05;
uniform float color_clip_max = .9;
uniform float color_clip_min = .1;
uniform float noise_add_multiplier = 0.1f;

float round_to_multiple(float value, float multiple)
{
	int div = int(round(value / multiple));
	return float(div) * multiple;
}


void fragment()
{
	vec4 screen_color = texture(SCREEN_TEXTURE, SCREEN_UV + vec2(blur_size, 0));
	screen_color += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(-blur_size, 0));
	screen_color += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(0, blur_size));
	screen_color += texture(SCREEN_TEXTURE, SCREEN_UV + vec2(0, -blur_size));
	screen_color /= 4.;

	vec2 noise_uv = SCREEN_UV * 3.;
	noise_uv = round(noise_uv * float(100)) / float(100);
	// noise_uv.x = round_to_multiple(noise_uv.x, .01);
	// noise_uv.y = round_to_multiple(noise_uv.y, .01);
	screen_color += (texture(noise, noise_uv - vec2(time_multiplier * TIME * .5)) - .5) * noise_add_multiplier;

	screen_color.r = round_to_multiple(screen_color.r, color_quantize_amount);
	screen_color.g = round_to_multiple(screen_color.g, color_quantize_amount);
	screen_color.b = round_to_multiple(screen_color.b, color_quantize_amount);

	screen_color.r = clamp(screen_color.r, color_clip_min, color_clip_max);
	screen_color.g = clamp(screen_color.g, color_clip_min, color_clip_max);
	screen_color.b = clamp(screen_color.b, color_clip_min, color_clip_max);

	screen_color.rgb += vec3((1. - color_clip_max) / 2.);

	if (texture(noise, SCREEN_UV + vec2(time_multiplier * TIME)).r > .9) screen_color.rgb = vec3(1.);
	COLOR = screen_color;
}
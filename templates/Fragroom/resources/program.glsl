@title: Title //Application special option. See examples/options

#version 330 core
out vec4 fragColor;
  
in vec2 fragCoord; // the input variable from the vertex shader (same name and same type)  
uniform float iTime; //Time, in seconds

void main(){
	float r = sin(iTime) * 0.5 + 0.5; //map the sine function to [0,1]
	float g = sin(iTime + 2*3.14*0.333) * 0.5 + 0.5; //map the sine function to [0,1] and introduces 120 degree phase
	float b = sin(iTime + 2*3.14*0.666) * 0.5 + 0.5; //map the sine function to [0,1] and introduces 240 degree phase
	
    fragColor = vec4(r, g, b, 1.0); //output
}
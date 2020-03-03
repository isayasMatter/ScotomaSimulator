# Scotoma Simulator

I have created the shader and script needed to simulate two scotomas (one for each eye) rendered as post render effects. Implmenting them as post render effects allows the scotomas to be rendered after everything is rendered and appear as if they are in or very close to the users eye. 

Ths shader and the attached script allows us to have granular control of the scotomas' behaviour including their size, feathering effect (so their edges appear natural), movement and color. The behaviour of the scotoma can be manipulated separately for each eye, or if you chose so, together for both eyes. Together, the above features allow you to customize the scotomas for each user, or even manipulate the behaviour of scotomas in the middle of an experiment. Moreover, you can add custom behaviours without the need to do big changes, for instance, showing scotomas only when they overlap can be implemented by manipulating the effect size in a single if block that checks the position of the two eyes.

## Installation

To use the ScotomaSimulator, just download it and copy the assets folder to your projects assets folder. Then, add the ScotomaSimulator.cs script to your main camera. Done!

## Usage

Most of the scotomas behaviours can be manipulated from the GUI without writting too much code.

<img src="Images/control-panel.png" width="50%">

To allow the scotomas to move with the user's eyes, all you need to do is to just modify the following lines of code in the Update method of the ScotomaSimulator class.

```c#
//Eye positions in normalized screen coordinates. You need to update these values from your eye tracker.
_leftEyePosition = new Vector2(0.5f,0.5f);
_rightEyePosition = new Vector2(0.5f,0.5f);
```

These two lines should be updated using the normalized eye coordinates from PupilLabs (or any other eye tracker). 


# Adaptive-solar-tree

The purpose of this project is to build an adaptive solar panel robotic system that is able to autonomously veer away from shadows and orient itself perpendicularly to the sun to maximize power efficiency.

![Solar Mechanism](Misc/Mechanism.png)

## Progress
- [X] Design branch mechanism
- [ ] Build physical branch mechanism
- [ ] Design physical computing system
- [ ] Build physical computing system
- [ ] Build 3D environment using Unity
- [ ] Train PPO agent using ML-Agents library
- [ ] Quantize and port trained ONNX model to Tensorflow-Lite

## Environment
### Observation space
- States of light sensors (int)
- Incident angle of branch panel (float)
### Action space
- Angular positions of all 3 stepper motors
### Rewards
- Reward will be given when all light sensor states are 1 and incident angles are approximately perpendicular to the sun
- Penalty will be given for every incremental angle to move motors

Environment scripts will be coded in C#.

## Orientation mechanism
A set of 3 stepper motors will be used for the mechanism. For rapid and cheap prototyping, LEGO parts will be used to interface with the motors. Stepper motors were used for its detent torque when powered off to retain its position unlike servo motors. The mechanism will consist of 2 revolute joints and 1 prismatic joint.

The angle values of the motors will have to be saved in flash memory during deep sleep.

## Physical computing
To further maximize power efficiency, a light-weight Raspberry Pi PICO microcontroller will be used for its physical computing. A deep sleep wake up mechanism using RTC will be incorporated to minimize power consumption in between periods of compute.

The microcontroller will be coded in C++ utilizing Tensorflow Lite API and Raspberry Pi PICO SDK.

Light sensors will be attached around the solar panels for light detection.

![System Diagram](Misc/Diagram.png)
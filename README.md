# Adaptive Solar Tree

## Progress
- [X] Design branch module
- [X] Design observation-decision-action-reward cycle
- [X] Build simulated environment using Unity
- [ ] Train various reinforcement architectures using ML-agents' Python API and StableBaselines (DQN, PPO, AC3)

## Environment
### Observation space
- Light sensor outputs (12 states, int)
<!-- - Camera feed gray image ((128, 128, 1) array, int) -->
- Optimal azimuth and zenith angle of module in degrees (2 states, float)
- Current motor angles in degrees (5 states, int)
### Action space
- Target angle of all motors in degrees (continuous action)
### Rewards
- +1 for every step when all light sensor states are 1 and incident angles are approximately perpendicular to the sun
- -0.00005 for every incremental angle to move motors

Environment scripts will be coded in C# in Unity.

## Orientation mechanism
Due to time-constraints, an actual mechanism was not built. Instead, a mock-up design taking inspiration from 
robotic arm manipulators was used as the mechanism namely the [Universal Robots' UR3 robotic arm](https://wiredworkers.io/universal-robots-ur3/).

The supposed mechanism would have 5-axis DOF, comprising of 5 rotary joints.

Off-the-shelf robotic arms tend to be equipped with servo or stepper motors that do not have high detent torque meaning that it cannot hold any torque when not powered. This characteristic will not make robotic arms work for this use case, as such, a self-locking mechanism will be required using worm gears.

## Physical computing
To further maximize power efficiency, a light-weight Raspberry Pi PICO microcontroller will be used for its physical computing. Deep sleep or dormant mode will be engaged in between periods of compute, reducing power consumptions up to 99% at 0.8mA. 

Light sensors will be attached around the solar panels for shadow detection.

Microcontroller scripts will be coded in C++ utilizing Tensorflow Lite API and Raspberry Pi PICO SDK.

![System Diagram](Misc/Diagram.png)

## Reinforcement Agent
The training scripts will be coded in Python using ML-Agents Python API and PyTorch.

## References
- [SPA C# Implementation](https://gist.github.com/paulhayes/54a7aa2ee3cccad4d37bb65977eb19e2)
- [NREL's Solar Position Algorithm (SPA)](https://midcdmz.nrel.gov/spa/)
- [On the Optimal Tilt Angle and Orientation of an On-Site Solar Photovoltaic Energy Generation System for Sabah’s Rural Electrification](https://doi.org/10.3390/su13105730)
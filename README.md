# Data Visualization in Virtual Reality - Fall 2018

## Motivation
This project seeks to build a framework for scientific user studies seeking to quantify which methods of data visualization are benefited the most by being displayed in a virtual reality context. 

It was produced for the Fall 2018 session of the Virtual Reality for Interdisciniplinary Applications course at Vanderbilt University by Robb Doering, Matthew Brady, Matthew Mahoney, John Kim, and Darren Ho. 

## Installation
### To Use / Test
If you are looking to use the project as it currently exists, all you must do is download the exe file stored in this repository and start it on Windows. To get the full effect one must use an Oculus Rift headset with the program, along with both controllers and a decently sized space to walk around in. 

### To Develop
If you are looking to edit this project and continue with development, you will need to follow these steps:
  1. Create an empty unity project.
  2. Follow the "Assets" section below to install all neccessary assets.
  3. Download the "MainScene.unity" file, import it into the project, and open it using File --> Open Scene.
  
#### Assets
  ##### * Resources Folder
    This can be found in this repository. To install, simply download into the Assets folder of your unity project.
  ##### * Oculus VR
    This is necessary to use the Oculus integration, and is found on the Unity Asset store under the name "Oculus Integration".
  ##### * Random From Distributions
    This library is used for generation of multivariate gaussians for use in the 3D scatterplots. Download from the Unity asset store
    under the name "Random From Distributions: Statistical Distributions Random Number" by Nathan Daly. 
  ##### * Real Materials
    These are materials used for the signs and control panels in the environment. Download from the Unity asset store under the name 
    "Real Materials Vol.0 [FREE]".
  ##### * LBRITE Font
    This font is used for the signs in the game. Download from the following link and put into your Assets folder: 
    https://brushez.com/lbrite-ttf.html.

## How To Use 
Once an Oculus Rift headset is plugged in and ready to go, press (T) on the keyboard to start the tutorial, (S) to start the 3D selection test, and (P) on the keyboard to start the pilot study. All data will be logged to the Results folder automatically for any completed runs of the pilot studies. To change parameters, such as how many graphs are shown in one pilot study or the central location of each graph, see the comments on the included code.

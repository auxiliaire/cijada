# cijada
Cross-platform 2D Cellular Automaton graphical modelling application (CijadA), written entirely in C# on the .NET platform, developed on Linux using the Mono runtime, while maintaining cross-platform compatibility with MS Windows.

The application calculates next-generation cell values in realtime so that it's capable of running Conway's Life Game as well as other reproduction cellular automatons by Stanislaw Ulam.

(When it was first developed, it was nicknamed as "carpet generator" because of its sometimes symmetrical patterns. The name stands for the combination of Cellular Automaton and the Arabic word for carpet - Sijada.)

Screenshot of the application running on linux mono/wine:
![Screenshot Linux](https://raw.githubusercontent.com/auxiliaire/cijada/master/Screenshot_20170628_220856.png)

Screenshot of the application running on Windows 10 (2018):
![Screenshot Windows](https://raw.githubusercontent.com/auxiliaire/cijada/master/Cijada-2018-Win10.png)

Screenshot of the application running on MacOS High Sierra (2018):
![Screenshot Mac](https://raw.githubusercontent.com/auxiliaire/cijada/master/Cijada-Mac-2018.png)

Screenshot of an edge case, where the game of life was running with aging, meaning that all the previous generations was kept as a non-interacting state on the board until they reached the age of extinction, when they were removed. After a while, aging was turned off, and the game of life continued to run normally but only in the areas that were empty (2019):

![Game Export](https://raw.githubusercontent.com/auxiliaire/cijada/master/Saves/closure-life.jpg)

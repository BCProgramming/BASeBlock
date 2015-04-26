<html><head><title>BASeBlock Help</title></head>


<body>
<img src="header.png" style="margin-left:auto;margin-right:auto">
<h2>BASeBlock Help Documentation</h2>
<p>
BASeBlock is an action game based on Arkanoid, which itself is based on breakout. The idea of these games is simple; keep bouncing a ball
back and forth until all the blocks are gone, aiming for a high score.
</p><p>
BASeBlock doesn't deviate much from this formula. However it has a number of mechanisms,power ups, and keystrokes that could use explaining.
</p><p>
<h3>The statusbar</h3>
<p>
<img src="sidebarimage.png" style="float:left"> The statusbar is shown on the right side of the game screen. It shows some useful information on your status:
<ul>
<li>Name of the Level<br>
This shows the name of the level- as well as which sequence number it is overall. Note that it's not strictly the case that all levels
will advance you sequentially, but it is a rule of thumb.</li>
<li>Score,Lives, HP,Energy<br>
Your Score increases as you acquire points. Scores are worth more when you have more objects on the screen. Your HP measures teh strength of the paddle. Explosions and enemy attacks will
chip away at this value. If it reaches zero, your paddle explodes. However, you do not lose a life until all the balls in play go below the bottom of the play field. 
</li>
<li>Energy<br>
The Energy bar shows how much "item" energy you have. Currently, the only power-up that utilizes this is the Magnet power up, which uses energy to apply
a force to all balls directly above the paddle.
</li>
<li>
Directly below the energy bar, the power-ups your paddle currently has are shown as small icons. In this case, the paddle in the game has the "sticky" behaviour, as well as magnet and Terminator (shooting) behaviours.
paddle "behaviours" typically indicate power-ups, but not always.
</li>
</ul>

<h3>Powerups</h3>
As blocks are destroyed, some of them will drop Powerups. these will fall from where they spawn. If you catch them with the paddle, they can cause all sortsof effects. Not all of them good.

<img src="..\images\addballpwr.png">


</p><p>
</body>

</html>
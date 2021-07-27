# Find-Edge-in-Image
A slow and non simple method to find edges in images.                        

This is probably the first most complicated thing I tried to make in Unity.  While it doesn't work well and the code is a mess, I feel like posting it here.

The original idea was to try and detect roads in 2D map images.  Basically you take a 2D image of a Map with roads and then this code would search through that 2d  data and find the things that looked like roads.  It would use specific colours and known attributes of roads to identify them and then turn that into point and spline data to be more efficiently used in a game or anywhere else you might like. 

It turns out it was a bit more complicated than I thought who would have guessed, so I only got as far as the edge and a bit along it.  The edge detection works ok though, all be it really slow but the problem is it gets stuck when moving along it.  Its because the data is too noisy along the edges, the code gets stuck in the cul de sacs of colours.  I couldn't figure it out at the time(I also kind of realised its redundant the data I needed could likely be gained better other ways).  I figure I may as well post this here someone else may find this useful(also I could free up some room on my computer).

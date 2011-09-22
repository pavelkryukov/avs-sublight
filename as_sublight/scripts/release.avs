LoadPlugin(".\..\bin\Release\as_sublight.dll")
clip = DirectShowSource("\\video.campus\video\all\!title\ghi\Incredibles [Суперсемейка] (2004)[animation]\ru\The Incredibles Суперсемейка.avi")
return Ambilight(clip)
#return clip
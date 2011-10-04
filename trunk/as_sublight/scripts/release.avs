LoadPlugin(".\..\bin\Release\as_sublight.dll")
clip = DirectShowSource("\\video.campus\video\all\by tag\collection\Monty Python\Monty Python and the Holy Grail [Монти Пайтон и священный Грааль] (1975)[movie]\Monty Python and the Holy Grail [eng].avi")
return Sublight(clip, 12050, IP="93.175.1.255")
#return clip
LoadPlugin(".\..\bin\Release\as_sublight.dll")
return Sublight(DirectShowSource("\\video.campus\video\movie\year\1971\A Clockwork Orange [Заводной апельсин] (1971)[movie]\en\A Clockwork Orange.avi"), PORT=9050)
#return clip
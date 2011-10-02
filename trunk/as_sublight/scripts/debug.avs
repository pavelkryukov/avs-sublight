LoadPlugin(".\..\bin\Debug\as_sublight.dll")
clip = DirectShowSource("\\video.campus\video\movie\year\1971\A Clockwork Orange [Заводной апельсин] (1971)[movie]\en\A Clockwork Orange.avi")
return Sublight(clip, 12050)
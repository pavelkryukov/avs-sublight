LoadPlugin(".\..\bin\Debug\as_sublight.dll")
clip = DirectShowSource(".\..\streams\test-1.wmv")
return Sublight(clip, 12050, IP="255.255.255.255")
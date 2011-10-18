Load_Stdcall_Plugin(".\..\bin\Release\as_sublight.dll")
clip = DirectShowSource(".\..\streams\test-1.wmv")
return Sublight(clip, 12050, IP="93.175.1.255")
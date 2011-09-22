// windows_ws.cpp : main project file.

using namespace System;
using System::Windows::Forms::Application;

#include "fullscreenform.h"

[STAThreadAttribute]
int main(array<System::String ^> ^args) {
	// Enabling Windows XP visual effects before any controls are created
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false); 

	// Create the main window and run it
	Application::Run(gcnew FullScreenForm(args->Length == 1)); 
	return 0;
}

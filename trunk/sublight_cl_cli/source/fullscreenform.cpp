#include <WinSock2.h>

using System::Windows::Forms::Application;

#include "fullscreenform.h"

FullScreenForm::FullScreenForm(bool side) : _side(side), _turn(false) {
    this->Size = System::Drawing::Size(300,300);
	this->Text = L"Form1";
	this->Padding = System::Windows::Forms::Padding(0);
//  this->AutoScaleDimensions = System::Drawing::SizeF(6F, 13F);
    this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::None;
	this->MaximizeBox = false;
    this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
    this->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &FullScreenForm::Click);
    this->BackColor = System::Drawing::Color::White;

    this->Top = 0;
    this->Left = 0;

    this->Height = 900;
    this->Width = 1440;
}

System::Void FullScreenForm::Click(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
    if (_turn) return;
    _turn = true;
    WSADATA data = {};
    WSAStartup (MAKEWORD (2, 2), &data);

    SOCKET sd = socket (AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    
    sockaddr_in addr = {};
    addr.sin_family = AF_INET;
    addr.sin_addr.S_un.S_addr = INADDR_ANY;
    addr.sin_port = 12050;
    bind (sd, (sockaddr*)&addr, sizeof sockaddr_in);

    if (_side) while (1) {

        unsigned char* buff = new unsigned char[8];
        recv (sd, (char*)buff, 8, 0);

        this->BackColor = System::Drawing::Color::FromArgb(buff[1], buff[2], buff[3]);

        Application::DoEvents();

        delete[] buff;
    }

    else while (1) {

        unsigned char* buff = new unsigned char[8];
        recv (sd, (char*)buff, 8, 0);

        this->BackColor = System::Drawing::Color::FromArgb(buff[5], buff[6], buff[7]);
         
        Application::DoEvents();

        delete[] buff;           
    }
    
    closesocket (sd);
}

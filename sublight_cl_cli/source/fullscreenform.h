
ref class FullScreenForm : public System::Windows::Forms::Form {
private:
    bool _side;
    bool _turn;
public:
    FullScreenForm(bool side);
    System::Void Click(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e);
};

   M S B A M L   `   `   `  ���� K  GMRMPConfigurator, Version=1.0.0.2, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:MRMPConfigurator.MRMPWCFServiceMRMPConfigurator.MRMPWCFService  ?$clr-namespace:MRMPConfigurator.FormsMRMPConfigurator.Forms  A%clr-namespace:MRMPConfigurator.ModelsMRMPConfigurator.Models  +    %MRMPConfigurator.Forms.CredentialForm   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     T PMahApps.Metro, Version=1.2.4.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4f;Controls,http://metro.mahapps.com/winfx/xaml/controls  :d2http://schemas.microsoft.com/expression/blend/2008  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  :MRPWCF-clr-namespace:MRMPConfigurator.MRMPWCFService  0local$clr-namespace:MRMPConfigurator.Forms  1model%clr-namespace:MRMPConfigurator.Models    � Icon$  
../mrp.ico��(  "MahApps.Metro.Controls.MetroWindow   EnableDWMDropShadow .  � WindowStartupLocation$ CenterScreen=� � Title$ Credential Form�� � SizeToContent$ WidthAndHeight=� � 
ResizeMode$ NoResize=����#FDFEFB +� FocusedElement ��  �� ElementName$ credential_description�� � 	Resources 	 �� MergedDictionaries
 �� Source�� 	 �� $
 /Resources/Icons.xaml?�%    .��� .+�� $��10q� � Orientation$ Vertical=�$��2��.+���  �� Orientation$ 
Horizontal=�.+�� -    �Name$ grid1��$
��Left=�$	��Top=� � ColumnDefinitions �� $
��Auto ��� $
��Auto � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��Description��$��0��$
��Left=�$��0��$��Center=�$��3,3,0,3q��� -    ��Name$ credential_description��$��1��$
��Left=�$��0��$��Center=�$	��220��$��3,4,0,4q�$��24������ *description+ �� Mode$ TwoWay=� �� NotifyOnValidationError .  �� ValidatesOnExceptions . �� $��Username��$��0��$
��Left=�$��1��$��Center=�$��3,3,0,3q��� -   $ credential_username��$��1��$
��Left=�$��1��$��Center=�$	��120��$��3,4,0,4q�$��24������ *
username+$ TwoWay=� .  . �� $��Password��$��0��$
��Left=�$��2��$��Center=�$��3,3,0,3q�G� -    G�Name$ credential_password��$��1��$��24��$��2��$
��Left=�$��3,4,0,4q�$	��130���� $��Domain��$��0��$
��Left=�$��3��$��Center=�$��3,3,0,3q��� -   $ credential_domain��$��1��$
��Left=�$��3��$��Center=�$	��120��$��3,4,0,4q�$��24������ *domain+$ TwoWay=� .  . �� $
��Type��$��0��$
��Left=�$��4��$��Center=�$��3,3,0,3q��� -    ��Name$ credential_type��$��1��$
��Left=�$��4��$��Center=�$	��120��$��3,4,0,4q�$��24�� �� SelectedValuePath$ id�� �� DisplayMemberPath$ credentialtype�� �� SelectedValue ��  �� Path$ credential_type�$ TwoWay=��� $��0,5,0,0q�$ 
Horizontal=�.+�� -   $ grid2��$
��Left=�$	��Top=� �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $��	0,10,0,10q��� $��Right=�$ 
Horizontal=�.+��� -    ��Name$ accept_credential�� �� 	IsDefault . .���� $ 
Horizontal=�.+��� $��15��$��15������ $ 
Foreground� �� RelativeSource �� *FindAncestor+ �� AncestorType" �� J� OpacityMask !�  j� Stretch$
 Fill=�   appbar_check  !� Visual#  �   �� $��5,0,0,0q�Accept�� -	   $ cancel_credential��! �� IsCancel! . $��15,0,0,0q�.���� $ 
Horizontal=�.+��� $��15��$��15������ $ 
Foreground� �� *FindAncestor+" �� !� $
 Fill=�  appbar_close#  �  �� $��5,0,0,0q�Cancel
   M S B A M L   `   `   `  ���� K  GMRMPConfigurator, Version=1.0.0.7, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:MRMPConfigurator.MRMPWCFServiceMRMPConfigurator.MRMPWCFService  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem ?$clr-namespace:MRMPConfigurator.FormsMRMPConfigurator.Forms  A%clr-namespace:MRMPConfigurator.ModelsMRMPConfigurator.Models  )    #MRMPConfigurator.Forms.PlatformForm   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     T PMahApps.Metro, Version=1.2.4.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4f;Controls,http://metro.mahapps.com/winfx/xaml/controls  :d2http://schemas.microsoft.com/expression/blend/2008  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  :MRPWCF-clr-namespace:MRMPConfigurator.MRMPWCFService  0sys&clr-namespace:System;assembly=mscorlib  0local$clr-namespace:MRMPConfigurator.Forms  1model%clr-namespace:MRMPConfigurator.Models    � Icon$  
../mrp.ico��(  "MahApps.Metro.Controls.MetroWindow   EnableDWMDropShadow .  � WindowStartupLocation$ CenterScreen=� � Title$ Platform Form�� � SizeToContent$ WidthAndHeight=� � 
ResizeMode$ NoResize=����#FDFEFB +� FocusedElement ��  �� ElementName$ platform_description�� � 	Resources 	 �� MergedDictionaries
 �� Source�� 	 �� $
 /Resources/Icons.xaml?�%    .��� $	��417��.+�� $��10q� � Orientation$ Vertical=�$��2��.+���  �� Orientation$ 
Horizontal=�.+�� -    �Name$ grid1��$
��Left=�$	��Top=� � ColumnDefinitions �� $
��Auto ��� $��127.667 ��� $��150.333 � � RowDefinitions �� $
��Auto �.+��� $	��100��$��Description��$��0��$
��Left=�$��3,3,0,3q�$��0��$��Center=��� -    ��Name$ platform_description��$��1��$
��Left=�$��24��$��	3,4,-64,4q�$��0��$��Center=�$	��287��$��2������ *description+ �� Mode$ TwoWay=� �� NotifyOnValidationError .  �� ValidatesOnExceptions . �� $��0,5,0,0q�$ 
Horizontal=�.+�� -   $ grid2��$
��Left=�$	��Top=� �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $��0,0,0,0q�$ 
Horizontal=�.+�� -   $ grid10��$
��Left=�$	��Top=� �� $
��Auto ��� $
��Auto � �� $
��Auto �.+��� $	��100��$��
Standalone��$��0��$
��Left=�$��3q�$��0��$��Center=��� $��0,10,0,0q�$��1�� d� 	IsChecked �� *
standalone+$ TwoWay=� .  . �� $��0,5,0,0q�$ 
Horizontal=�.+�� -   $ grid3��$
��Left=�$	��Top=� �� $
��Auto ��� $
��Auto � �� $
��Auto �.+��� $	��100��$��Vendor��$��0��$
��Left=�$��3q�$��0��$��Center=��� -    ��Name$ platform_vendor��$��1��$
��Left=�$
��Auto��$��	3,5,-63,5q�$��0��$��Center=�$	��186�� �� SelectedValuePath$ ID�� �� DisplayMemberPath$ Vendor�� �� SelectedValue ��  �� Path$ vendor�$ TwoWay=��� $��0,5,0,0q�$ 
Horizontal=�.+�� -   $ grid4��$
��Left=�$	��Top=� �� $
��Auto ��� $
��Auto � �� $
��Auto �.+��� $	��100��$��Target��$��0��$
��Left=�$��3q�$��0��$��Center=��� -   $ platform_url��$��1��$
��Left=�$��24��$��	3,4,-77,4q�$��0��$��Center=�$	��280������ *url+$ TwoWay=� .  . �� -	   $  platform_dimensiondata_url��$��1��$
��Left=�$
��Auto��$��3q�$��0��$��Center=�$	��120��$	 Url��$ Description�� �� $	 url�$ TwoWay=��� $��0,5,0,0q�$ 
Horizontal=�.+�� -
   $ grid6��$
��Left=�$	��Top=� �� $
��Auto ��� $
��Auto � �� $
��Auto �.+��� $	��100��$��
Credential��$��0��$
��Left=�$��3q�$��0��$��Center=��� -   $ platform_credential��$��1��$
��Left=�$��23��$��3q�$��0��$	��120��$ id��$ description�� �� $ credential_id�$ TwoWay=��� -    ��Name$ test_connection_button��$��10,0,0,0q�$��23��.���� $ 
Horizontal=�.+��� $��15������ $ 
Foreground� �� RelativeSource �� *FindAncestor+ �� AncestorType" �� J� OpacityMask !�  j� Stretch$
 Fill=�   appbar_camera_flash !� Visual# �   �� $��5,0,0,0q�Test�� $��0,5,0,0q�$ 
Horizontal=�.+�� -   $ grid7��$
��Left=�$	��Top=� �� $
��Auto ��� $��14 ��� $	��120 � �� $
��Auto �.+��� $	��100��$��
Datacenter��$��0��$
��Left=�$��3,3,0,3q�$��0��$��Center=��� -   $ platform_datacenter��$��1��$
��Auto��$��6,5,-9,5q�$��0��$��Center=�$
 moid��$ 
datacenter��$��2�� �� $
 moid�$ TwoWay=��� -     ��Name$!  current_platform_datacenter��$
��Left=�$
��Wrap=�$	��Top=�$��2��$��163,7,-177,0q�$	��106������ *
datacenter+�� $��	0,10,0,10q��� $��Right=�$ 
Horizontal=�$	��397��.+��� -   $  message��$
��Left=�$	��229��$��0,0,30,0q��� -   $ accept_platform��! �� 	IsDefault! . $��0,0,0,0q�.���� $ 
Horizontal=�.+��� $��15��$��15������ $ 
Foreground� �� *FindAncestor+" �� !� $
 Fill=�  appbar_check# �  �� $��5,0,0,0q�Accept�� -   $ cancel_platform��" �� IsCancel" . $��15,0,0,0q�.���� $ 
Horizontal=�.+��� $��15��$��16������ *
Foreground+ �� *FindAncestor+" �� !� $
 Fill=�  appbar_close# �  �� $��5,0,0,0q��� $�Cancel��
   M S B A M L   `   `   `  ���� K  GMRMPConfigurator, Version=1.0.0.7, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:MRMPConfigurator.MRMPWCFServiceMRMPConfigurator.MRMPWCFService  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem ?$clr-namespace:MRMPConfigurator.FormsMRMPConfigurator.Forms  A%clr-namespace:MRMPConfigurator.ModelsMRMPConfigurator.Models  )    #MRMPConfigurator.Forms.WorkloadForm   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     T PMahApps.Metro, Version=1.2.4.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4f;Controls,http://metro.mahapps.com/winfx/xaml/controls  :d2http://schemas.microsoft.com/expression/blend/2008  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  :MRPWCF-clr-namespace:MRMPConfigurator.MRMPWCFService  0sys&clr-namespace:System;assembly=mscorlib  0local$clr-namespace:MRMPConfigurator.Forms  1model%clr-namespace:MRMPConfigurator.Models    � Icon$  
../mrp.ico��(  "MahApps.Metro.Controls.MetroWindow   EnableDWMDropShadow .  � WindowStartupLocation$ CenterScreen=� � Title$ Workload Form�� � SizeToContent$ WidthAndHeight=� � 
ResizeMode$ NoResize=����#FDFEFB +� FocusedElement ��  �� ElementName$ hostname�� � 	Resources 	 �� MergedDictionaries
 �� Source�� 	 �� $
 /Resources/Icons.xaml?�%    .��� $	��417��.+�� $��10q� � Orientation$ Vertical=�$��2��.+���  �� Orientation$ 
Horizontal=�.+�� -    �Name$ grid1��$
��Left=�$	��Top=� � ColumnDefinitions �� $
��Auto ��� $
��Auto � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $	��100��$��Hostname��$��0��$
��Left=�$��3,3,0,3q�$��0��$��Center=��� -    ��Name$ workload_hostname��$��1��$
��Left=�$��24��$��	3,4,-64,4q�$��0��$��Center=�$	��287��$��2������ *
hostname+ �� Mode$ TwoWay=� �� NotifyOnValidationError .  �� ValidatesOnExceptions . �� $	��100��$��IP List��$��0��$
��Left=�$��3,3,0,3q�$��1��$��Center=��� -   $ workload_iplist��$��1��$
��Left=�$��24��$��	3,4,-64,4q�$��1��$��Center=�$	��287��$��2������ *iplist+$ TwoWay=� .  . �� $	��100��$��
Credential��$��0��$
��Left=�$��3q�$��3��$��Center=��� -    ��Name$ workload_credential��$��1��$
��Left=�$��23��$��3q�$��3��$	��220�� �� SelectedValuePath$ id�� �� DisplayMemberPath$ description�� �� SelectedValue ��  �� Path$ credential_id�$ TwoWay=��� $��	0,10,0,10q��� $��Right=�$ 
Horizontal=�$	��397��.+��� -    ��Name$ message��$
��Left=�$	��229��$��0,0,30,0q��� -    ��Name$ accept_platform�� �� 	IsDefault . $��0,0,0,0q�.���� $ 
Horizontal=�.+��� $��15��$��15������ $ 
Foreground� �� RelativeSource �� *FindAncestor+ �� AncestorType" �� J� OpacityMask !�  j� Stretch$
 Fill=�   appbar_check  !� Visual#  �   �� $��5,0,0,0q�Accept�� -   $ cancel_platform��! �� IsCancel! . $��15,0,0,0q�.���� $ 
Horizontal=�.+��� $��15��$��16������ *
Foreground+ �� *FindAncestor+" �� !� $
 Fill=�  appbar_close#  �  �� $��5,0,0,0q��� $�Cancel��
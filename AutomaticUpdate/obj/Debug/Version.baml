   M S B A M L   `   `   `  ����J  FAutomaticUpdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:AutomaticUpdateAutomaticUpdate  5   	       AutomaticUpdate.Version   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   )localclr-namespace:AutomaticUpdate  5   	     � Title$  Version��5	   	   $	��400��6   $	��300��6&    � WindowStyle$
 None=�62    � AllowsTransparency .  � ShowInTaskbar .  .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Loaded��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *ShowCommand+5       � 	Resources 5         VersionItem	 �� Orientation
 �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers��%�  &	        '�� =    �� 5   
   .R�5      �� $	 
Horizontal=�6   ���	#FF2D2D30$��30��6I   $��	-22,5,0,0q�6U   $
��Left=�6h   .+�5      �� $��30��6   �� 5      
�������$��Center=�6R   ��6   �� *VersionDetailItem+6R   �� 5      $��10,0,0,0q�6   $��4��6.   
���  ��68   5      5      �� 5   
   "
 ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      ��  5      5!      5#      5$      5%      .��5'      � ���	#FF1E1E1E � ColumnDefinitions 5(   
   �� 5)      $��0.5 * �6   �� 5*      $	��99* �6   .+�5,   
   � $��0��6   
���  ��6   � 5-   
   $��1��6   .+�5.      �� -   5         ��Name$ VersionList��5.      ���	#FF1E1E1E$��
0,50,10,20q�6B   ���	#FF1E1E1E$��Hidden=�5/      #��[  6J   50      51      52      53      
   M S B A M L   `   `   `  ����J  FAutomaticUpdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:AutomaticUpdateAutomaticUpdate  5   	   %clr-namespace:PrintS.UC	PrintS.UC  5   	        AutomaticUpdate.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   )localclr-namespace:AutomaticUpdate  5   	    ucclr-namespace:PrintS.UC  5   	     � Title$  QH自动更新程序��5
   	   $	��525��6   $	��400��6&    � WindowStyle$
 None=�63    � AllowsTransparency .  � WindowStartupLocation$ CenterScreen=�6`    � Icon$&  @..\..\Resource\Image\图标.png��6�   .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Loaded��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *LoadCommand+5      	 � 	Resources	 5       
  Color1 
 Color2��%V   &	        &	 !     ��5   
   	#FF2B3585��6*   ��5   
   	#FF2A3452��6*   5      
 � TaskbarItemInfo
 5      *  $System.Windows.Shell.TaskbarItemInfo  5   
      Description$, &An app with a taskbar info description��6      ProgressValue 5      �� *TaskbarValue+   ProgressState 6=   �� *TaskbarState+.��5      � ���	#FF1E1E1E � RowDefinitions 5   
   �� 5      $��8* �6   6*   �� 5      $
��0.5* �6   6,   �� 5       $��6* �6   6*   �� 5!      $��3* �6   6*   �� 5"      $��6* �6   6*   �� 5#      $	��38* �6   6+   �� 5$      $
��0.2* �6   6,   �� 5%      $��6* �6   6*   .+�5(   
   � -   5        $��0��5(      ���	#FF2D2D30 J� 	AllowDrop . .+�5)      �� $
��Left=�6   $��Center=�63   $��16(�6N   $��Microsoft Yi Baiti)�6\   
�������$��10,0,0,0q�5*   +   QH自动更新程序6G   �� 5+      $
��auto��6   $��Right=�6&    �� Orientation$ 
Horizontal=�6B   .+�5,      �� -   5        $��40��5,      
�������$��0q�6N   $��22(�6Y   $
��Bold"�6g   ���	#FF2D2D30���	#FF2D2D30$��Right=�5-   H   $��DengXian)�6d   ��5,   y   �� *MinimizeWindowCommand+.��5-   z   -6}   �� 5.      -   5        $��40��5.      $��0q�68   $��20(�6C   
����������	#FF2D2D30���	#FF2D2D30$��Right=�5/   1   .��6M   x6P   50      51      � 53   
   $��1��6   
���  ��6/   � 55   
   $��2��6   ���	#FF1E1E1E.+�56      �� -   5        $��fileDate��56      $��Center=�6(   $
��Left=�6C   $��10,0,0,0q�6^   
�������$��Microsoft JhengHei)�57      ���	#FF1E1E1E��6S   �� *FileNameText+�� 58      -   5        $
��Rate��58      $��Center=�6$   $��Right=�6?   $��0,0,80,0q�6[   
�������$��Microsoft JhengHei)�59      ���	#FF1E1E1E��6S   �� *Rate+�� 5:      -   5        $
��Time��5:      $��Center=�6$   $��	10,0,10,0q�6?   
�������$��Microsoft JhengHei)�5;      ���	#FF1E3581$��Right=�6S   ��6o   �� *Time+5<      � 5>   
   $��3��6   .+�5?      �� ���	#FF0F0F0F �� CornerRadius$ 10,10,10,10��6,   $
��10,0q�6G   ._�5@      � -   5        $��pb��5@      ���	#FF131313���	#FF131313���	#FF686868 � Value 6n   �� *	PBValue+6�   5A      5B      � 5C   
   $��4��6   ���	#FF1E1E1E.+�5D      �� $ 
Horizontal=�6   $��Right=�62   .+�5E      �� -   5        $��VersionNumber��5E      $��Center=�61   $��Right=�6L   
�������$��0,0,10,0q�5F      ��6.   �� *VersionText+y�6K   �� *VersionVisibility+�� 5G      -	   5        $��VersionDetail��5G      $��Center=�61   $��Right=�6L   ���	#FF6CCAD5$��0,0,20,0q�5H      $��新版功能��6U   y�6,   �� *VersionVisibility+ J� InputBindings 5I      d� 5J       d� MouseAction$ 	LeftClicke�6K    �� Command 6(   �� *VersionCommand+6K   5L      5M      5N      � 5�   
   $��5��6   $��	10,0,10,0q�6   ���	#FF1E1E1E.+�5�         PrintS.UC.RollImg  -
   5          Name$ rollImg��5�      $
��Left=�5�      $��0,0,0,0q�6)   $	��Top=�6:   $
��Auto��6R   $
��Auto��6`   5�      � 5�   
   $��6��6   
���  ��60   � 5�   
   $��7��6   .+�5�      �� -   5         ��Name$ Begin��5�      ���	#FF686868���	#FF686868
�������$��0,0,15,0q�5�      $��Right=�6'   $��Center=�6C   $��14(�6^   ��5�      �� *ButtonContent+ �� TargetNullValue$ 开始��5�      5�      5�      
   M S B A M L   `   `   `  ����  Flower_shop    Flower_shop.InputDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Ввод данных��5   	   $	��200��5   	   $	��400��5   	    � WindowStartupLocation$ CenterOwner=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency . 
������ .��5      �� 
������� �� CornerRadius$ 10��5      $��10q�5      $��1q�5      ���	#FFE4E4E4 J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 10Q�6      ShadowDepth$ 0Q�5         Color$ #FFB6C1��5      ._�5   
   � $��20q�6   	 � RowDefinitions	 5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      �� -   5        
 ��Name$
 
PromptText��5      $&�� Введите значение:��5      $��16(�5       $��0,0,0,10q�5!      �� 5#      -   5         ��Name$ ResponseTextBox��5#      $��1��5$         ModernTextBox#��[  5%      $��40��5&      $	��Top=�5'      �� 5)      $��2��6    �� Orientation$ 
Horizontal=�5*      $��Right=�5+      $��0,10,0,0q�5,      .+�5-      �� -   5        $��OK��5-        ModernButton#��[ 5.      $	��100��5/      $��35��50      $��0,0,10,0q�51      52      �� 53      -   5        $��Отмена��53      #��[ 54      $	��100��55      $��35��56      57      58      59      5:      5;      
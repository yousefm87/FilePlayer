AutoItSetOption ("TrayIconDebug", 1)
Local $hWnd = WinWait($CmdLine[1], "", 30)
WinActivate($hWnd)
WinWaitActive($hWnd, "", 30)
SendKeepActive($hWnd)
Send("{AltDown}{Enter}{AltUp}")
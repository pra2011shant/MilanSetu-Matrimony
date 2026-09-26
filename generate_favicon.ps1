Add-Type -AssemblyName System.Drawing

$bmp = New-Object System.Drawing.Bitmap 64, 64
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)

# Heart polygon points
$points = @(
    (New-Object System.Drawing.Point 32, 58),
    (New-Object System.Drawing.Point 14, 42),
    (New-Object System.Drawing.Point 4, 28),
    (New-Object System.Drawing.Point 6, 14),
    (New-Object System.Drawing.Point 18, 6),
    (New-Object System.Drawing.Point 32, 18),
    (New-Object System.Drawing.Point 46, 6),
    (New-Object System.Drawing.Point 58, 14),
    (New-Object System.Drawing.Point 60, 28),
    (New-Object System.Drawing.Point 50, 42)
)

$brush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 225, 29, 72)) # Crimson #e11d48
$g.FillPolygon($brush, $points)

$pen = New-Object System.Drawing.Pen ([System.Drawing.Color]::White), 2
$g.DrawPolygon($pen, $points)

# Highlight spark
$sparkBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(200, 255, 255, 255))
$g.FillEllipse($sparkBrush, 16, 14, 8, 6)

$g.Dispose()

# Save PNGs
$bmp.Save("e:\Work\MilanSetu\Frontend\MilanSetu.UI\public\favicon.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Save("e:\Work\MilanSetu\Frontend\MilanSetu.UI\src\favicon.png", [System.Drawing.Imaging.ImageFormat]::Png)

$hIcon = $bmp.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($hIcon)

$fs1 = New-Object System.IO.FileStream "e:\Work\MilanSetu\Frontend\MilanSetu.UI\public\favicon.ico", ([System.IO.FileMode]::Create)
$icon.Save($fs1)
$fs1.Close()

$fs2 = New-Object System.IO.FileStream "e:\Work\MilanSetu\Frontend\MilanSetu.UI\src\favicon.ico", ([System.IO.FileMode]::Create)
$icon.Save($fs2)
$fs2.Close()

$bmp.Dispose()
Write-Host ">>> Red Heart Icon Generated Successfully in public/ & src/! <<<"

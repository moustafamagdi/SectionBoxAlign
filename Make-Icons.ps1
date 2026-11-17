Add-Type -AssemblyName System.Drawing
$folder=Join-Path $PSScriptRoot 'payload\MMTools.FloatingTools.bundle\Contents\Align1.1.0\Icons'
foreach($kind in @('alignface','aligndir')) {
 foreach($size in @(16,32)) {
  $bitmap=New-Object Drawing.Bitmap($size,$size)
  $g=[Drawing.Graphics]::FromImage($bitmap)
  $g.SmoothingMode='AntiAlias'
  $g.ScaleTransform(($size/32.0),($size/32.0))
  $blue=New-Object Drawing.Pen([Drawing.Color]::FromArgb(20,102,158),1.6)
  $orange=New-Object Drawing.Pen([Drawing.Color]::FromArgb(235,135,25),2.4)
  $fill=New-Object Drawing.SolidBrush([Drawing.Color]::FromArgb(99,198,236))
  if($kind -eq 'alignface') {
   $points=[Drawing.PointF[]]@([Drawing.PointF]::new(5,11),[Drawing.PointF]::new(20,5),[Drawing.PointF]::new(20,22),[Drawing.PointF]::new(5,28))
   $g.FillPolygon($fill,$points);$g.DrawPolygon($blue,$points)
   $g.DrawLine($blue,20,5,27,9);$g.DrawLine($blue,27,9,27,26);$g.DrawLine($blue,27,26,20,22)
   $g.DrawLine($orange,2,4,16,4);$g.DrawLine($orange,16,4,12,1);$g.DrawLine($orange,16,4,12,7)
  } else {
   $g.DrawRectangle($blue,4,11,23,15)
   $g.DrawLine($orange,5,23,26,5);$g.DrawLine($orange,26,5,19,6);$g.DrawLine($orange,26,5,25,12)
   $g.FillEllipse($fill,2,20,6,6);$g.DrawEllipse($blue,2,20,6,6)
   $g.FillEllipse($fill,17,7,5,5);$g.DrawEllipse($blue,17,7,5,5)
  }
  $bitmap.Save((Join-Path $folder ($kind+$size+'.png')),[Drawing.Imaging.ImageFormat]::Png)
  $g.Dispose();$bitmap.Dispose();$blue.Dispose();$orange.Dispose();$fill.Dispose()
 }
}


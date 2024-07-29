﻿using SkiaSharp;

namespace CoreLibrary.Tools;

public static class TextToImageConverter
{
    public static string ConvertSingleLineToBase64(string text, int textSize = 16)
    {
        //指定字体黑体，将给定的string转化成Base64编码的图片
        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Left,
            TextSize = textSize
        };

        var bounds = new SKRect();
        textPaint.MeasureText(text, ref bounds);

        var bitmap = new SKBitmap((int)Math.Ceiling(bounds.Width), (int)Math.Ceiling(bounds.Height));
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        canvas.DrawText(text, -bounds.Left, -bounds.Top, textPaint);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return Convert.ToBase64String(data.ToArray());
    }

    public static string ConvertToBase64(string text, int textSize = 16)
    {
        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Left,
            TextSize = textSize,
            Typeface = SKTypeface.FromFamilyName("黑体")
        };

        var lines = text.Split('\n');
        var maxWidth = 0f;
        var totalHeight = 0f;

        foreach (var line in lines)
        {
            var bounds = new SKRect();
            textPaint.MeasureText(line, ref bounds);
            maxWidth = Math.Max(maxWidth, bounds.Width) * 1.01f;
            totalHeight += textSize + 5; // 假设行间距为5
        }

        var bitmap = new SKBitmap((int)Math.Ceiling(maxWidth), (int)Math.Ceiling(totalHeight));
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        var y = textSize;
        foreach (var line in lines)
        {
            canvas.DrawText(line, 0, y, textPaint);
            y += textSize + 5; // 更新y坐标，为下一行文本做准备
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return Convert.ToBase64String(data.ToArray());
    }
}
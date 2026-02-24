using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Chip8.Avalonia;

public sealed class Chip8DisplayControl : Control
{
    private const int ScreenWidth = 64;
    private const int ScreenHeight = 32;

    public static readonly StyledProperty<byte[]?> FrameBufferProperty =
        AvaloniaProperty.Register<Chip8DisplayControl, byte[]?>(nameof(FrameBuffer));

    static Chip8DisplayControl()
    {
        AffectsRender<Chip8DisplayControl>(FrameBufferProperty);
    }

    public byte[]? FrameBuffer
    {
        get => GetValue(FrameBufferProperty);
        set => SetValue(FrameBufferProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.FillRectangle(Brushes.Black, new Rect(Bounds.Size));

        byte[]? buffer = FrameBuffer;
        if (buffer is null || buffer.Length < ScreenWidth * ScreenHeight || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        double pixelWidth = Bounds.Width / ScreenWidth;
        double pixelHeight = Bounds.Height / ScreenHeight;

        for (int y = 0; y < ScreenHeight; y++)
        {
            for (int x = 0; x < ScreenWidth; x++)
            {
                if (buffer[(y * ScreenWidth) + x] == 0)
                {
                    continue;
                }

                var rect = new Rect(
                    x * pixelWidth,
                    y * pixelHeight,
                    Math.Ceiling(pixelWidth),
                    Math.Ceiling(pixelHeight));
                context.FillRectangle(Brushes.White, rect);
            }
        }
    }
}

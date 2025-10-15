// using System;
// using System.IO;

// namespace JKTechnologies.CommonPackage.Utilities
// {

//     public class SeensioGifDecoder
//     {

//         public enum Status { Ok, FormatError, OpenError }

//         public class GifFrame
//         {

//             public byte[] data;
//             public float delay;

//             public GifFrame(byte[] data, float delay)
//             {
//                 this.data = data;
//                 this.delay = delay;
//             }
//         }

//         #region Public Fields

//         public int TotalNumberOfFrames { get; private set; }
//         public bool AllFramesDecoded { get; private set; }

//         #endregion

//         #region Internal Fields

//         private Stream inStream;
//         private Status status;

//         private int width;
//         private int height;
//         private bool gctFlag;
//         private int gctSize;
//         private int loopCount = 1;

//         private int[] gct;
//         private int[] lct;
//         private int[] act;

//         private int bgIndex;
//         private int bgColor;
//         private int lastBgColor;

//         private bool lctFlag;
//         private bool interlace;
//         private int lctSize;

//         private int ix, iy, iw, ih;
//         private int lrx, lry, lrw, lrh;
//         private int[] image;
//         private byte[] bitmap;

//         private readonly byte[] block = new byte[256];
//         private int blockSize;


//         private int dispose;

//         private int lastDispose;
//         private bool transparency;
//         private float delay;
//         private int transIndex;
//         private long imageDataOffset; // start of image data in stream

//         private const int MaxStackSize = 4096;

//         private short[] prefix;
//         private byte[] suffix;
//         private byte[] pixelStack;
//         private byte[] pixels;

//         //protected ArrayList frames; // frames read from current file
//         //protected bool CacheFrames;
//         private GifFrame currentFrame;
//         private int frameCount;

//         #endregion


//         #region Public API

//         public float GetCurrentFrameDelay()
//         {
//             return currentFrame.delay;
//         }

//         public int GetFrameCount()
//         {
//             return frameCount;
//         }

//         public GifFrame GetCurrentFrame()
//         {
//             return currentFrame;
//         }

//         public int GetLoopCount()
//         {
//             return loopCount;
//         }

//         public int GetImageWidth()
//         {
//             return width;
//         }

//         public int GetImageHeight()
//         {
//             return height;
//         }

//         public Status Read(Stream inStream)
//         {
//             Init();
//             if (inStream != null)
//             {
//                 this.inStream = inStream;
//                 ReadHeader();
//                 if (Error())
//                 {
//                     status = Status.FormatError;
//                 }
//             }
//             else
//             {
//                 status = Status.OpenError;
//             }
//             return status;
//         }

//         public void Reset()
//         {
//             inStream.Position = 0;
//             Read(inStream);
//         }

//         public void Close()
//         {
//             inStream.Dispose();

//         }

//         public void ReadContents(bool loop)
//         {
//             while (!Error())
//             {
//                 var code = Read();
//                 switch (code)
//                 {
//                     // Image Separator
//                     case 0x2C:
//                         ReadImage();
//                         return;
//                     // Extension
//                     case 0x21:
//                         code = Read();
//                         switch (code)
//                         {
//                             // Graphics Control Extension
//                             case 0xf9:
//                                 ReadGraphicControlExt();
//                                 break;
//                             // Application Extension
//                             case 0xff:
//                                 ReadBlock();

//                                 string app = "";
//                                 for (var i = 0; i < 11; i++)
//                                 {
//                                     app += (char)block[i];
//                                 }

//                                 if (app.Equals("NETSCAPE2.0"))
//                                 {
//                                     ReadNetscapeExt();
//                                 }
//                                 else
//                                 {
//                                     Skip();
//                                 }

//                                 break;
//                             // Uninteresting Extension
//                             default:
//                                 Skip();
//                                 break;
//                         }
//                         break;
//                     // Terminator
//                     case 0x3b:
//                         TotalNumberOfFrames = frameCount;
//                         if (loop)
//                         {
//                             ResetReader();
//                             break;
//                         }
//                         AllFramesDecoded = true;
//                         return;
//                     // Bad Byte
//                     case 0x00:
//                         break;
//                     default:
//                         status = Status.FormatError;
//                         break;
//                 }
//             }
//         }

//         #endregion

//         #region Helper Methods

//         private void ResetReader()
//         {
//             frameCount = 0;
//             AllFramesDecoded = false;
//             inStream.Position = imageDataOffset;
//         }


//         private void SetPixels()
//         {
//             if (lastDispose > 0)
//             {
//                 var n = frameCount - 1;
//                 if (n > 0)
//                 {
//                     if (lastDispose == 2)
//                     {
//                         var fillcolor = transparency ? 0 : lastBgColor;
//                         for (var i = 0; i < lrh; i++)
//                         {
//                             var line = i;
//                             line += lry;
//                             if (line >= height) continue;
//                             var linein = height - line - 1;
//                             var dx = linein * width + lrx;
//                             var endx = dx + lrw;
//                             while (dx < endx)
//                             {
//                                 image[dx++] = fillcolor;
//                             }
//                         }
//                     }
//                 }
//             }

//             var pass = 1;
//             var inc = 8;
//             var iline = 0;
//             for (var i = 0; i < ih; i++)
//             {
//                 var line = i;
//                 if (interlace)
//                 {
//                     if (iline >= ih)
//                     {
//                         pass++;
//                         switch (pass)
//                         {
//                             case 2:
//                                 iline = 4;
//                                 break;
//                             case 3:
//                                 iline = 2;
//                                 inc = 4;
//                                 break;
//                             case 4:
//                                 iline = 1;
//                                 inc = 2;
//                                 break;
//                         }
//                     }
//                     line = iline;
//                     iline += inc;
//                 }
//                 line += iy;
//                 if (line >= height) continue;

//                 var sx = i * iw;
//                 var linein = height - line - 1;
//                 var dx = linein * width + ix;
//                 var endx = dx + iw;

//                 for (; dx < endx; dx++)
//                 {
//                     var c = act[pixels[sx++] & 0xff];
//                     if (c != 0)
//                     {
//                         image[dx] = c;
//                     }
//                 }
//             }
//         }

//         /**
//          * Decodes LZW image data into pixel array.
//          * Adapted from John Cristy's ImageMagick.
//          */
//         private void DecodeImageData()
//         {
//             const int nullCode = -1;
//             var npix = iw * ih;
//             int bits,
//                 code,
//                 count,
//                 i, first,
//                 top,
//                 bi;

//             if ((pixels == null) || (pixels.Length < npix))
//             {
//                 pixels = new byte[npix];
//             }
//             if (prefix == null) prefix = new short[MaxStackSize];
//             if (suffix == null) suffix = new byte[MaxStackSize];
//             if (pixelStack == null) pixelStack = new byte[MaxStackSize + 1];

//             var dataSize = Read();
//             var clear = 1 << dataSize;
//             var endOfInformation = clear + 1;
//             var available = clear + 2;
//             var oldCode = nullCode;
//             var codeSize = dataSize + 1;
//             var codeMask = (1 << codeSize) - 1;
//             for (code = 0; code < clear; code++)
//             {
//                 prefix[code] = 0;
//                 suffix[code] = (byte)code;
//             }

//             var datum = bits = count = first = top = bi = 0;

//             for (i = 0; i < npix;)
//             {
//                 if (top == 0)
//                 {
//                     for (; bits < codeSize; bits += 8)
//                     {
//                         if (count == 0)
//                         {
//                             count = ReadBlock();
//                             bi = 0;
//                         }
//                         datum += (block[bi++] & 0xff) << bits;
//                         count--;
//                     }

//                     code = datum & codeMask;
//                     datum >>= codeSize;
//                     bits -= codeSize;

//                     if ((code > available) || (code == endOfInformation))
//                         break;
//                     if (code == clear)
//                     {
//                         codeSize = dataSize + 1;
//                         codeMask = (1 << codeSize) - 1;
//                         available = clear + 2;
//                         oldCode = nullCode;
//                         continue;
//                     }
//                     if (oldCode == nullCode)
//                     {
//                         pixelStack[top++] = suffix[code];
//                         oldCode = code;
//                         first = code;
//                         continue;
//                     }
//                     var inCode = code;
//                     if (code == available)
//                     {
//                         pixelStack[top++] = (byte)first;
//                         code = oldCode;
//                     }

//                     for (; code > clear; code = prefix[code])
//                     {
//                         pixelStack[top++] = suffix[code];
//                     }

//                     first = (suffix[code]) & 0xff;

//                     if (available >= MaxStackSize)
//                         break;
//                     pixelStack[top++] = (byte)first;
//                     prefix[available] = (short)oldCode;
//                     suffix[available] = (byte)first;
//                     available++;
//                     if (((available & codeMask) == 0)
//                         && (available < MaxStackSize))
//                     {
//                         codeSize++;
//                         codeMask += available;
//                     }
//                     oldCode = inCode;
//                 }

//                 top--;
//                 pixels[i++] = pixelStack[top];
//             }

//             for (; i < npix; i++)
//             {
//                 pixels[i] = 0;
//             }
//         }

//         private bool Error()
//         {
//             return status != Status.Ok;
//         }

//         private void Init()
//         {
//             status = Status.Ok;
//             frameCount = 0;
//             currentFrame = null;
//             AllFramesDecoded = false;
//             gct = null;
//             lct = null;
//         }

//         private int Read()
//         {
//             var dataByte = 0;
//             try
//             {
//                 dataByte = inStream.ReadByte();
//             }
//             catch (IOException)
//             {
//                 status = Status.FormatError;
//             }
//             return dataByte;
//         }

//         private int ReadBlock()
//         {
//             blockSize = Read();
//             var n = 0;
//             if (blockSize <= 0) return n;
//             try
//             {
//                 while (n < blockSize)
//                 {
//                     var count = inStream.Read(block, n, blockSize - n);
//                     if (count == -1)
//                         break;
//                     n += count;
//                 }
//             }
//             catch (IOException)
//             {
//             }

//             if (n < blockSize)
//             {
//                 status = Status.FormatError;
//             }
//             return n;
//         }

//         private int[] ReadColorTable(int ncolors)
//         {
//             var nbytes = 3 * ncolors;
//             int[] tab = null;
//             var c = new byte[nbytes];
//             var n = 0;
//             try
//             {
//                 n = inStream.Read(c, 0, c.Length);
//             }
//             catch (IOException)
//             {
//             }
//             if (n < nbytes)
//             {
//                 status = Status.FormatError;
//             }
//             else
//             {
//                 tab = new int[256];
//                 var i = 0;
//                 var j = 0;
//                 while (i < ncolors)
//                 {
//                     uint r = (c[j++]);
//                     var g = (c[j++]) & (uint)0xff;
//                     var b = (c[j++]) & (uint)0xff;
//                     tab[i++] = (int)(0xff000000 | (b << 16) | (g << 8) | r);
//                 }
//             }
//             return tab;
//         }

//         private void ReadGraphicControlExt()
//         {
//             Read();
//             var packed = Read();
//             dispose = (packed & 0x1c) >> 2;
//             if (dispose == 0)
//             {
//                 dispose = 1;
//             }
//             transparency = (packed & 1) != 0;
//             delay = ReadShort() / 100f;
//             transIndex = Read();
//             Read();
//         }

//         private void ReadHeader()
//         {
//             var id = "";
//             for (var i = 0; i < 6; i++)
//             {
//                 id += (char)Read();
//             }
//             if (!id.StartsWith("GIF", StringComparison.CurrentCulture))
//             {
//                 status = Status.FormatError;
//                 return;
//             }

//             ReadLsd();
//             if (gctFlag && !Error())
//             {
//                 gct = ReadColorTable(gctSize);
//                 bgColor = gct[bgIndex];
//             }
//             imageDataOffset = inStream.Position;
//         }

//         private void ReadImage()
//         {
//             ix = ReadShort();
//             iy = ReadShort();
//             iw = ReadShort();
//             ih = ReadShort();

//             var packed = Read();
//             lctFlag = (packed & 0x80) != 0;
//             interlace = (packed & 0x40) != 0;
//             lctSize = 2 << (packed & 7);

//             if (lctFlag)
//             {
//                 lct = ReadColorTable(lctSize);
//                 act = lct;
//             }
//             else
//             {
//                 act = gct;
//                 if (bgIndex == transIndex)
//                     bgColor = 0;
//             }
//             var save = 0;
//             if (transparency)
//             {
//                 save = act[transIndex];
//                 act[transIndex] = 0;
//             }

//             if (act == null)
//             {
//                 status = Status.FormatError;
//             }

//             if (Error()) return;

//             DecodeImageData();
//             Skip();

//             if (Error()) return;

//             if (image == null) image = new int[width * height];
//             if (bitmap == null) bitmap = new byte[width * height * sizeof(int)];
//             SetPixels();

//             Buffer.BlockCopy(image, 0, bitmap, 0, bitmap.Length);
//             currentFrame = new GifFrame(bitmap, delay);

//             frameCount++;

//             if (transparency)
//             {
//                 act[transIndex] = save;
//             }
//             ResetFrame();
//         }

//         private void ReadLsd()
//         {
//             width = ReadShort();
//             height = ReadShort();

//             var packed = Read();
//             gctFlag = (packed & 0x80) != 0;
//             gctSize = 2 << (packed & 7);

//             bgIndex = Read();
//             Read();
//         }

//         private void ReadNetscapeExt()
//         {
//             do
//             {
//                 ReadBlock();
//                 if (block[0] != 1) continue;
//                 var b1 = block[1] & 0xff;
//                 var b2 = block[2] & 0xff;
//                 loopCount = (b2 << 8) | b1;
//             } while ((blockSize > 0) && !Error());
//         }

//         private int ReadShort()
//         {
//             return Read() | (Read() << 8);
//         }

//         private void ResetFrame()
//         {
//             lastDispose = dispose;
//             lrx = ix;
//             lry = iy;
//             lrw = iw;
//             lrh = ih;
//             lastBgColor = bgColor;
//             lct = null;
//         }

//         private void Skip()
//         {
//             do
//             {
//                 ReadBlock();
//             } while ((blockSize > 0) && !Error());
//         }

//         #endregion
//     }
// }
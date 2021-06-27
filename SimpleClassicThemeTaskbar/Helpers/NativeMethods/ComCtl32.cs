using System;
using System.Runtime.InteropServices;

using static SimpleClassicThemeTaskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class ComCtl32
    {
        /// <summary>
        /// Draws text that has a shadow.
        /// </summary>
        /// <param name="pszText">A pointer to a string that contains the text to be drawn.</param>
        /// <param name="cch">A <see langword="uint"></see> that specifies the number of characters in the string that is to be drawn.</param>
        /// <param name="prc">A pointer to a RECT structure that contains, in logical coordinates, the rectangle in which the text is to be drawn.</param>
        /// <param name="dwFlags">A DWORD that specifies how the text is to be drawn. See Format Values for possible parameter values.</param>
        /// <param name="crText">A COLORREF structure that contains the color of the text.</param>
        /// <param name="crShadow">A COLORREF structure that contains the color of the text shadow.</param>
        /// <param name="ixOffset">A value of type int that specifies the x-coordinate of where the text should begin.</param>
        /// <param name="iyOffset">A value of type int that specifies the y-coordinate of where the text should begin.</param>
        /// <returns>Returns the height of the text in logical units if the function succeeds, otherwise returns zero.</returns>
        [DllImport(nameof(ComCtl32), CharSet = CharSet.Unicode)]
        internal static extern int DrawShadowText(IntPtr hdc, string pszText, uint cch, RECT prc, uint dwFlags, uint crText, uint crShadow, int ixOffset, int iyOffset);
    }
}

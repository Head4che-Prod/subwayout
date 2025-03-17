using System;

namespace Prefabs.Puzzles.Hanoi
{
    public class HanoiException : Exception
    {
        public HanoiException(string s) : base(s) { }
    }
}
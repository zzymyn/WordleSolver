using System.Runtime.CompilerServices;

namespace WordleSolver
{
    public partial struct Word
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckAnswer(Word gs, Word ss, Word ans)
        {
            if (gs.C0 == ss.C0)
            {
                if (ans.C0 != '2')
                    return false;
                gs.C0 = '?';
                ss.C0 = '!';
            }
            else if (ans.C0 == '2')
            {
                return false;
            }

            if (gs.C1 == ss.C1)
            {
                if (ans.C1 != '2')
                    return false;
                gs.C1 = '?';
                ss.C1 = '!';
            }
            else if (ans.C1 == '2')
            {
                return false;
            }

            if (gs.C2 == ss.C2)
            {
                if (ans.C2 != '2')
                    return false;
                gs.C2 = '?';
                ss.C2 = '!';
            }
            else if (ans.C2 == '2')
            {
                return false;
            }

            if (gs.C3 == ss.C3)
            {
                if (ans.C3 != '2')
                    return false;
                gs.C3 = '?';
                ss.C3 = '!';
            }
            else if (ans.C3 == '2')
            {
                return false;
            }

            if (gs.C4 == ss.C4)
            {
                if (ans.C4 != '2')
                    return false;
                gs.C4 = '?';
                ss.C4 = '!';
            }
            else if (ans.C4 == '2')
            {
                return false;
            }


            if (gs.C0 == ss.C1)
            {
                if (ans.C0 != '1')
                    return false;
                ss.C1 = '!';
            }
            else if (gs.C0 == ss.C2)
            {
                if (ans.C0 != '1')
                    return false;
                ss.C2 = '!';
            }
            else if (gs.C0 == ss.C3)
            {
                if (ans.C0 != '1')
                    return false;
                ss.C3 = '!';
            }
            else if (gs.C0 == ss.C4)
            {
                if (ans.C0 != '1')
                    return false;
                ss.C4 = '!';
            }
            else if (ans.C0 == '1')
            {
                return false;
            }

            if (gs.C1 == ss.C0)
            {
                if (ans.C1 != '1')
                    return false;
                ss.C0 = '!';
            }
            else if (gs.C1 == ss.C2)
            {
                if (ans.C1 != '1')
                    return false;
                ss.C2 = '!';
            }
            else if (gs.C1 == ss.C3)
            {
                if (ans.C1 != '1')
                    return false;
                ss.C3 = '!';
            }
            else if (gs.C1 == ss.C4)
            {
                if (ans.C1 != '1')
                    return false;
                ss.C4 = '!';
            }
            else if (ans.C1 == '1')
            {
                return false;
            }

            if (gs.C2 == ss.C0)
            {
                if (ans.C2 != '1')
                    return false;
                ss.C0 = '!';
            }
            else if (gs.C2 == ss.C1)
            {
                if (ans.C2 != '1')
                    return false;
                ss.C1 = '!';
            }
            else if (gs.C2 == ss.C3)
            {
                if (ans.C2 != '1')
                    return false;
                ss.C3 = '!';
            }
            else if (gs.C2 == ss.C4)
            {
                if (ans.C2 != '1')
                    return false;
                ss.C4 = '!';
            }
            else if (ans.C2 == '1')
            {
                return false;
            }

            if (gs.C3 == ss.C0)
            {
                if (ans.C3 != '1')
                    return false;
                ss.C0 = '!';
            }
            else if (gs.C3 == ss.C1)
            {
                if (ans.C3 != '1')
                    return false;
                ss.C1 = '!';
            }
            else if (gs.C3 == ss.C2)
            {
                if (ans.C3 != '1')
                    return false;
                ss.C2 = '!';
            }
            else if (gs.C3 == ss.C4)
            {
                if (ans.C3 != '1')
                    return false;
                ss.C4 = '!';
            }
            else if (ans.C3 == '1')
            {
                return false;
            }

            if (gs.C4 == ss.C0)
            {
                if (ans.C4 != '1')
                    return false;
                ss.C0 = '!';
            }
            else if (gs.C4 == ss.C1)
            {
                if (ans.C4 != '1')
                    return false;
                ss.C1 = '!';
            }
            else if (gs.C4 == ss.C2)
            {
                if (ans.C4 != '1')
                    return false;
                ss.C2 = '!';
            }
            else if (gs.C4 == ss.C3)
            {
                if (ans.C4 != '1')
                    return false;
                ss.C3 = '!';
            }
            else if (ans.C4 == '1')
            {
                return false;
            }

            return true;
        }
    }
}
namespace WordleSolver
{
    public partial struct Word
    {
        public static Word GetAnswer(Word gs, Word ss)
        {
            var ans = new Word('0', '0', '0', '0', '0');

            if (gs.C0 == ss.C0)
            {
                gs.C0 = '?';
                ss.C0 = '!';
                ans.C0 = '2';
            }
            if (gs.C1 == ss.C1)
            {
                gs.C1 = '?';
                ss.C1 = '!';
                ans.C1 = '2';
            }
            if (gs.C2 == ss.C2)
            {
                gs.C2 = '?';
                ss.C2 = '!';
                ans.C2 = '2';
            }
            if (gs.C3 == ss.C3)
            {
                gs.C3 = '?';
                ss.C3 = '!';
                ans.C3 = '2';
            }
            if (gs.C4 == ss.C4)
            {
                gs.C4 = '?';
                ss.C4 = '!';
                ans.C4 = '2';
            }

            if (gs.C0 == ss.C0)
            {
                gs.C0 = '?';
                ss.C0 = '!';
                ans.C0 = '1';
            }
            else if (gs.C0 == ss.C1)
            {
                gs.C0 = '?';
                ss.C1 = '!';
                ans.C0 = '1';
            }
            else if (gs.C0 == ss.C2)
            {
                gs.C0 = '?';
                ss.C2 = '!';
                ans.C0 = '1';
            }
            else if (gs.C0 == ss.C3)
            {
                gs.C0 = '?';
                ss.C3 = '!';
                ans.C0 = '1';
            }
            else if (gs.C0 == ss.C4)
            {
                gs.C0 = '?';
                ss.C4 = '!';
                ans.C0 = '1';
            }
            if (gs.C1 == ss.C0)
            {
                gs.C1 = '?';
                ss.C0 = '!';
                ans.C1 = '1';
            }
            else if (gs.C1 == ss.C1)
            {
                gs.C1 = '?';
                ss.C1 = '!';
                ans.C1 = '1';
            }
            else if (gs.C1 == ss.C2)
            {
                gs.C1 = '?';
                ss.C2 = '!';
                ans.C1 = '1';
            }
            else if (gs.C1 == ss.C3)
            {
                gs.C1 = '?';
                ss.C3 = '!';
                ans.C1 = '1';
            }
            else if (gs.C1 == ss.C4)
            {
                gs.C1 = '?';
                ss.C4 = '!';
                ans.C1 = '1';
            }
            if (gs.C2 == ss.C0)
            {
                gs.C2 = '?';
                ss.C0 = '!';
                ans.C2 = '1';
            }
            else if (gs.C2 == ss.C1)
            {
                gs.C2 = '?';
                ss.C1 = '!';
                ans.C2 = '1';
            }
            else if (gs.C2 == ss.C2)
            {
                gs.C2 = '?';
                ss.C2 = '!';
                ans.C2 = '1';
            }
            else if (gs.C2 == ss.C3)
            {
                gs.C2 = '?';
                ss.C3 = '!';
                ans.C2 = '1';
            }
            else if (gs.C2 == ss.C4)
            {
                gs.C2 = '?';
                ss.C4 = '!';
                ans.C2 = '1';
            }
            if (gs.C3 == ss.C0)
            {
                gs.C3 = '?';
                ss.C0 = '!';
                ans.C3 = '1';
            }
            else if (gs.C3 == ss.C1)
            {
                gs.C3 = '?';
                ss.C1 = '!';
                ans.C3 = '1';
            }
            else if (gs.C3 == ss.C2)
            {
                gs.C3 = '?';
                ss.C2 = '!';
                ans.C3 = '1';
            }
            else if (gs.C3 == ss.C3)
            {
                gs.C3 = '?';
                ss.C3 = '!';
                ans.C3 = '1';
            }
            else if (gs.C3 == ss.C4)
            {
                gs.C3 = '?';
                ss.C4 = '!';
                ans.C3 = '1';
            }
            if (gs.C4 == ss.C0)
            {
                gs.C4 = '?';
                ss.C0 = '!';
                ans.C4 = '1';
            }
            else if (gs.C4 == ss.C1)
            {
                gs.C4 = '?';
                ss.C1 = '!';
                ans.C4 = '1';
            }
            else if (gs.C4 == ss.C2)
            {
                gs.C4 = '?';
                ss.C2 = '!';
                ans.C4 = '1';
            }
            else if (gs.C4 == ss.C3)
            {
                gs.C4 = '?';
                ss.C3 = '!';
                ans.C4 = '1';
            }
            else if (gs.C4 == ss.C4)
            {
                gs.C4 = '?';
                ss.C4 = '!';
                ans.C4 = '1';
            }

            return ans;
        }
    }
}
namespace WordleSolver
{
    internal partial class Program
    {
        private static Word GetAnswer(Word gs, Word ss)
        {
            var rs = new Word('0', '0', '0', '0', '0');

            var v0 = gs.C0 == ss.C0;
            if (v0)
                rs.C0 = '2';
            var v1 = gs.C1 == ss.C1;
            if (v1)
                rs.C1 = '2';
            var v2 = gs.C2 == ss.C2;
            if (v2)
                rs.C2 = '2';
            var v3 = gs.C3 == ss.C3;
            if (v3)
                rs.C3 = '2';
            var v4 = gs.C4 == ss.C4;
            if (v4)
                rs.C4 = '2';

            if (!v0 && gs.C0 == ss.C0)
            {
                rs.C0 = '1';
                v0 = true;
            }
            else if (!v1 && gs.C0 == ss.C1)
            {
                rs.C0 = '1';
                v1 = true;
            }
            else if (!v2 && gs.C0 == ss.C2)
            {
                rs.C0 = '1';
                v2 = true;
            }
            else if (!v3 && gs.C0 == ss.C3)
            {
                rs.C0 = '1';
                v3 = true;
            }
            else if (!v4 && gs.C0 == ss.C4)
            {
                rs.C0 = '1';
                v4 = true;
            }
            if (!v0 && gs.C1 == ss.C0)
            {
                rs.C1 = '1';
                v0 = true;
            }
            else if (!v1 && gs.C1 == ss.C1)
            {
                rs.C1 = '1';
                v1 = true;
            }
            else if (!v2 && gs.C1 == ss.C2)
            {
                rs.C1 = '1';
                v2 = true;
            }
            else if (!v3 && gs.C1 == ss.C3)
            {
                rs.C1 = '1';
                v3 = true;
            }
            else if (!v4 && gs.C1 == ss.C4)
            {
                rs.C1 = '1';
                v4 = true;
            }
            if (!v0 && gs.C2 == ss.C0)
            {
                rs.C2 = '1';
                v0 = true;
            }
            else if (!v1 && gs.C2 == ss.C1)
            {
                rs.C2 = '1';
                v1 = true;
            }
            else if (!v2 && gs.C2 == ss.C2)
            {
                rs.C2 = '1';
                v2 = true;
            }
            else if (!v3 && gs.C2 == ss.C3)
            {
                rs.C2 = '1';
                v3 = true;
            }
            else if (!v4 && gs.C2 == ss.C4)
            {
                rs.C2 = '1';
                v4 = true;
            }
            if (!v0 && gs.C3 == ss.C0)
            {
                rs.C3 = '1';
                v0 = true;
            }
            else if (!v1 && gs.C3 == ss.C1)
            {
                rs.C3 = '1';
                v1 = true;
            }
            else if (!v2 && gs.C3 == ss.C2)
            {
                rs.C3 = '1';
                v2 = true;
            }
            else if (!v3 && gs.C3 == ss.C3)
            {
                rs.C3 = '1';
                v3 = true;
            }
            else if (!v4 && gs.C3 == ss.C4)
            {
                rs.C3 = '1';
                v4 = true;
            }
            if (!v0 && gs.C4 == ss.C0)
            {
                rs.C4 = '1';
                v0 = true;
            }
            else if (!v1 && gs.C4 == ss.C1)
            {
                rs.C4 = '1';
                v1 = true;
            }
            else if (!v2 && gs.C4 == ss.C2)
            {
                rs.C4 = '1';
                v2 = true;
            }
            else if (!v3 && gs.C4 == ss.C3)
            {
                rs.C4 = '1';
                v3 = true;
            }
            else if (!v4 && gs.C4 == ss.C4)
            {
                rs.C4 = '1';
                v4 = true;
            }

            return rs;
        }
    }
}
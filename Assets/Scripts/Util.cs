public static class Util{
    public static string ToQuat(this ushort value, int length = 8){
        var sb = new System.Text.StringBuilder();
        while(length-- > 0){
            var n = value % 4;
            sb.Insert(0, n);
            value /= 4;
        }
        string result = sb.ToString();
        return result;
    }
}
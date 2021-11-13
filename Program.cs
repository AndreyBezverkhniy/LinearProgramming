using System;
using System.IO;

class Utils{
    public static bool DoesArrayConsistsElement(int[] arr,int element){
        for(int i=0;i<arr.Length;i++){
            if(arr[i]==element){
                return true;
            }
        }
        return false;
    }
    public static int ReadIntFromFile(StreamReader file){
        return int.Parse(file.ReadLine());
    }
    public static double[] ReadSequenceFromFile(StreamReader file,int size){
        string str;
        string[] strArr;
        double[] array = new double[size];
        str = file.ReadLine();
        strArr = str.Split(" ");
        array = new double[size];
        for(int i=0;i<size;i++){
            array[i]=double.Parse(strArr[i]);
        }
        return array;
    }
    public static void ReadSystemFromFile(StreamReader file,
    out double[][] A,out double[] b,int lines,int variableCount){
        string str;
        string[] strArr;
        A = new double[lines][];
        b = new double[lines];
        for(int i=0;i<lines;i++){
            str = file.ReadLine();
            strArr = str.Split(" ");
            A[i]=new double[variableCount];
            b[i] = double.Parse(strArr[variableCount]);
            for(int j=0;j<variableCount;j++){
                A[i][j]=double.Parse(strArr[j]);
            }
        }
    }
    public static void copyArrays(ref double[] dst,double[] src){
        dst=new double[src.Length];
        for(int i=0;i<src.Length;i++){
            dst[i]=src[i];
        }
    }
    public static void copyArrays(ref int[] dst,int[] src){
        dst=new int[src.Length];
        for(int i=0;i<src.Length;i++){
            dst[i]=src[i];
        }
    }
    public static bool isNullVector(double[] v){
        for(int i=0;i<v.Length;i++){
            if(v[i]!=0){
                return false;
            }
        }
        return true;
    }
    public static void PrintVector(double[] v){
        if(v.Length>0){
            Console.Write("{0}",v[0]);
            for(int i=1;i<v.Length;i++){
                Console.Write(" {0}",v[i]);
            }
        }
        Console.WriteLine();
    }
    public static bool isInt(double x){
        return x==(double)((int)x);
    }
}

class Standartic{
    public int n; // кол-во оригинальных переменных
    public int m; // кол-во неравенств-ограничений
    public double[][] A; // матрица системы неравенств
    public double[] b; // свободные члены (правая часть неравенств)
    public double[] c; // вектор коэффициентов в целевой функции
    public Standartic(){} // по умолчанию ничего не существует
    public Standartic(Standartic copy):this(copy.n,copy.m){
        for(int i=0;i<b.Length;i++){
            b[i]=copy.b[i];
        }
        for(int i=0;i<c.Length;i++){
            c[i]=copy.c[i];
        }
        for(int i=0;i<A.Length;i++){
            for(int j=0;j<A[i].Length;j++){
                A[i][j]=copy.A[i][j];
            }
        }

    }
    public Standartic(int _n,int _m){
        n=_n;
        m=_m;
        b=new double[_m];
        c=new double[_n];
        A=new double[_m][];
        for(int i=0;i<_m;i++){
            A[i]=new double[_n];
        }
    }
    public void LoadFromFile(string path){
        StreamReader file;
        file = new StreamReader(path);
        n = Utils.ReadIntFromFile(file);
        m = Utils.ReadIntFromFile(file);
        c = Utils.ReadSequenceFromFile(file,n);
        Utils.ReadSystemFromFile(file,out A,out b,m,n);
        file.Close();
    }
    public void Print(){
        //Console.WriteLine("Standartic");
        Console.Write("z = ");
        for(int i=0;i<n;i++){
            if (i>0){
                Console.Write(" + ");
            }
            Console.Write("({0})*x{1}",c[i],i);
        }
        Console.WriteLine();
        for(int i=0;i<m;i++){
            for(int j=0;j<n;j++){
                if (j>0){
                    Console.Write(" + ");
                }
                Console.Write("({0})*x{1}",A[i][j],j);
            }
            Console.WriteLine(" <= {0}",b[i]);
        }
    }
    public Standartic getAUX(){
        Standartic aux = new Standartic();
        aux.n=n+1;
        aux.m=m;
        aux.b = new double[m];
        for(int i=0;i<m;i++){
            aux.b[i]=b[i];
        }
        aux.c = new double[n+1];
        for(int i=0;i<n;i++){
            aux.c[i]=0;
        }
        aux.c[n]=-1;
        aux.A=new double[m][];
        for(int i=0;i<m;i++){
            aux.A[i]=new double[n+1];
            for(int j=0;j<n;j++){
                aux.A[i][j]=A[i][j];
            }
            aux.A[i][n]=-1;
        }
        return aux;
    }
    public int getMinBIndex(){
        int index_min_b=0;
        for(int i=0;i<m;i++){
            if(b[index_min_b]>b[i]){
                index_min_b=i;
            }
        }
        return index_min_b;
    }
    public int getAdditionalVariableIndexAUX(){
        // usage only in aux-ed Standartic
        return n-1;
    }
    public bool checkXAllowability(double[] X){
        for(int i=0;i<m;i++){
            double left=0;
            for(int j=0;j<n;j++){
                left+=A[i][j]*X[j];
            }
            if(left>b[i]){
                return false;
            }
        }
        return true;
    }
    public void addRestriction(int variable,string inequality,double number){
        double sign;
        if(inequality=="<="){
            sign=+1;
        } else if(inequality==">="){
            sign=-1;
        } else {
            throw new Exception(); // forbidden or unknown inequality
        }
        int _m=m+1;
        double[] _b=new double[_m];
        double[][] _A=new double[_m][];
        for(int i=0;i<m;i++){
            _b[i]=b[i];
            _A[i]=A[i];
        }
        _b[_m-1]=number*sign;
        _A[_m-1]=new double[n];
        for(int i=0;i<n;i++){
            if(i==variable){
                _A[_m-1][i]=sign;
            } else {
                _A[_m-1][i]=0;
            }
        }
        A=_A;
        b=_b;
        m=_m;
    }
    public uint getHash(){
        double hash=0;
        double basis=3;
        double multiplier=1;
        hash+=multiplier*n;
        multiplier=(multiplier>256)?1:(multiplier*basis);
        hash+=multiplier*m;
        multiplier=(multiplier>256)?1:(multiplier*basis);
        for(int i=0;i<n;i++){
            hash+=multiplier*c[i];
            multiplier=(multiplier>256)?1:(multiplier*basis);
        }
        for(int i=0;i<m;i++){
            hash+=multiplier*b[i];
            multiplier=(multiplier>256)?1:(multiplier*basis);
        }
        for(int i=0;i<m;i++){
            for(int j=0;j<n;j++){
                hash+=multiplier*A[i][j];
                multiplier=(multiplier>256)?1:(multiplier*basis);
            }
        }
        return (uint)hash;
    }
}

class Canonic{
    public int n; // кол-во оригинальных и дополнительных переменных
    public int m; // кол-во неравенств-ограничений
    public int[] B; // множество базисных переменных
    public double[][] A; // матрица новой системы неравенств
    public double[] b; // свободные члены (правая часть неравенств)
    public double[] c; // новый вектор коэффициентов в целевой функции
    public double v; // значение целевой функции при 0-х небазисных переменных
    public Canonic(){} // по умолчанию ничего не существует
    public Canonic(Canonic copy):this(copy.n,copy.m){
        v=copy.v;
        for(int i=0;i<B.Length;i++){
            B[i]=copy.B[i];
        }
        for(int i=0;i<b.Length;i++){
            b[i]=copy.b[i];
        }
        for(int i=0;i<c.Length;i++){
            c[i]=copy.c[i];
        }
        for(int i=0;i<A.Length;i++){
            for(int j=0;j<A[i].Length;j++){
                A[i][j]=copy.A[i][j];
            }
        }

    }
    public Canonic(Standartic standartic){
        m = standartic.m; // кол-во неравенств тоже
        n = standartic.n + standartic.m; // оригинальные и дополнительные
        v = 0;
        B = new int[standartic.m]; // сначала: дополнительные переменные
        for(int i=0;i<standartic.m;i++) {
            B[i]=standartic.n+i;
        }
        A = new double[standartic.m][]; // расширение матрицы стандартной формы
        for (int i=0;i<standartic.m;i++) {
            A[i] = new double[standartic.n+standartic.m];
            // перенос в правую часть с изменением знака
            for(int j=0;j<standartic.n;j++){
                A[i][j]=-standartic.A[i][j];
            }
            for(int j=standartic.n;j<standartic.n+standartic.m;j++){
                A[i][j]=0;
            }
        }
         // добавление 0-х коэффициентов при дополнительных переменных
        c = new double[standartic.n+standartic.m];
        for(int i=0;i<standartic.n;i++){
            c[i]=standartic.c[i];
        }
        for(int i=standartic.n;i<standartic.n+standartic.m;i++){
            c[i]=0;
        }
        b = new double[standartic.m]; // вектор свободных членов такой же
        for(int i=0;i<standartic.m;i++){
            b[i]=standartic.b[i];
        }
    }
    public Canonic(Standartic standartic,Canonic canonic_aux)
    :this(canonic_aux.n-1,canonic_aux.m){
        // задачи должны совпадать по кол-ву исходных переменных и ограничений
        if(canonic_aux.n!=standartic.n+standartic.m+1 ||
        canonic_aux.m!=standartic.m){
            throw new Exception();
        }
        // при необходимости выводим дополнительную переменную из базиса
        int additionalVariable=standartic.getAUX().getAdditionalVariableIndexAUX();
        int entering=-1;
        int leaving;
        if(Utils.DoesArrayConsistsElement(canonic_aux.B,additionalVariable)){
            leaving=additionalVariable;
            int equality=canonic_aux.equalityByBasic(additionalVariable);
            for(int i=0;i<canonic_aux.n;i++){
                if(canonic_aux.A[equality][i]>0){
                    entering=i;
                    break;
                }
            }
            canonic_aux = canonic_aux.Pivot(leaving,entering);
        }
        // избавляемcя от дополнительной переменной в ограничениях
        for(int j=0;j<m;j++){
            for(int i=0;i<n;i++){
                if(i<additionalVariable){
                    A[j][i]=canonic_aux.A[j][i];
                }
                if(i>additionalVariable){
                    A[j][i-1]=canonic_aux.A[j][i];
                }
            }
        }
        // берём неизменные данные
        Utils.copyArrays(ref b,canonic_aux.b);
        Utils.copyArrays(ref B,canonic_aux.B);
        // избавляемcя от дополнительной переменной в функции
        v=0;
        for(int i=0;i<n;i++){
            if(i<additionalVariable){
                c[i]=canonic_aux.c[i];
            }
            if(i>additionalVariable){
                c[i-1]=canonic_aux.c[i];
            }
        }
        // избавляемся от базовых переменных в функции
        for(int i=0;i<n;i++){
            if(Utils.DoesArrayConsistsElement(B,i)){
                // базовая перемнная i используется в z
                int equation=equalityByBasic(i);
                substituteVariableInEquation(i,A[equation],b[equation],ref c,ref v);
            }
        }
    }
    public Canonic(int _n,int _m){
        if(_n<=_m){
            throw new Exception();
        }
        n=_n;
        m=_m;
        b=new double[_m];
        c=new double[_n];
        A=new double[_m][];
        for(int i=0;i<_m;i++){
            A[i]=new double[_n];
        }
        B=new int[_m];
    }
    public void Print(){
        //Console.WriteLine("Canonic");
        Console.Write("z = {0}",v);
        for(int i=0;i<n;i++){
            Console.Write(" + ({0})*x{1}",c[i],i);
        }
        Console.WriteLine();
        for(int i=0;i<m;i++){
            Console.Write("x{0} = {1}",B[i],b[i]);
            for(int j=0;j<n;j++){
                Console.Write(" + ({0})*x{1}",A[i][j],j);
            }
            Console.WriteLine();
        }
    }
    public int equalityByBasic(int vIndex){
        for(int i=0;i<m;i++){
            if(B[i]==vIndex){
                return i;
            }
        }
        return -1;
    }
    private void exchangeVariablesInEquation(int leaving,int entering,
    ref double[] coefficients,ref double freeMember){
        if(coefficients[entering]==0){
            throw new Exception();
        }
        freeMember/=-coefficients[entering];
        coefficients[leaving]=1.0/coefficients[entering];
        for(int i=0;i<n;i++){
            if(i==leaving || i==entering){
                continue;
            }
            coefficients[i]/=-coefficients[entering];
        }
        coefficients[entering]=0;
    }
    private void substituteVariableInEquation(int variable,
    double[] vCoefficients,double vFreeMember,ref double[] coefficients,
    ref double freeMember){
        freeMember+=vFreeMember*coefficients[variable];
        for(int i=0;i<n;i++){
            if(i==variable){
                continue;
            }
            coefficients[i]+=vCoefficients[i]*coefficients[variable];
        }
        coefficients[variable]=0;
    }
    private void PivotRecalculateRestrictions(ref Canonic result,int leaving,
    int entering){
        int equality = equalityByBasic(leaving);
        result.B[equality]=entering;
        exchangeVariablesInEquation(leaving,entering,ref result.A[equality],
        ref result.b[equality]);
        for(int i=0;i<m;i++){
            if(i==equality){
                continue;
            }
            substituteVariableInEquation(entering,result.A[equality],result.b[equality],
            ref result.A[i],ref result.b[i]);
        }
    }
    private void PivotRecalculateFunction(ref Canonic result,int leaving,
    int entering){
        int equation = equalityByBasic(leaving);
        substituteVariableInEquation(entering,result.A[equation],
        result.b[equation],ref result.c,ref result.v);
    }
    public Canonic Pivot(int leaving,int entering){
        Canonic result=new Canonic(this);
        PivotRecalculateRestrictions(ref result,leaving,entering);
        PivotRecalculateFunction(ref result,leaving,entering);
        return result;
    }
    public double getVariableNValue(int index){
        for(int i=0;i<m;i++){
            if(B[i]==index){
                return b[i];
            }
        }
        return 0;
    }
    public double[] getXValue(){
        double[] res = new double[n-m];
        for(int i=0;i<n-m;i++){
            res[i]=getVariableNValue(i);
        }
        return res;
    }
    public double getFunctionValue(){
        return v;
    }
    public int getOriginalVariableAmount(){
        return n-m;
    }
}

class SimplexAnswer{
    public Canonic canonic;
    public string type;
    public SimplexAnswer(Canonic _canonic){
        canonic = _canonic;
        type = "solved";
    }
    public SimplexAnswer(string _type){
        canonic = new Canonic();
        type = _type;
    }
    public void PrintAnswer(){
        for(int i=0;i<canonic.n;i++){
            Console.Write("x{0}={1} ",i,canonic.getVariableNValue(i));
        }
        Console.WriteLine();
    }
}

class Solution{
    public static SimplexAnswer Simplex(Standartic standartic){
        // поиск начального допустимого решения
        SimplexAnswer startingSolution=Initialize(standartic);
        if(startingSolution.type!="solved"){
            return startingSolution;
        }
        // поиск оптимального решения
        Canonic canonic=startingSolution.canonic;
        for(;;){
            // поиск вводимой переменной
            int e=-1;
            for(int i=0;i<canonic.n;i++){
                if(canonic.c[i]>0){
                    e=i;
                    break;
                }
            }
            // если вводимой переменной нет, достигнуто оптимальное решение
            if(e==-1){
                break;
            }
            // поиск возможных приращения вводимой переменной
            double[] delta=new double[canonic.m];
            for(int equality=0;equality<canonic.B.Length;equality++){
                if(canonic.A[equality][e]<0){
                    delta[equality]=-canonic.b[equality]/canonic.A[equality][e];
                } else {
                    delta[equality]=double.PositiveInfinity;
                }
            }
            // поиск выводимой переменной
            int l_equality=0;
            for(int i=1;i<canonic.B.Length;i++){
                if(delta[i]<delta[l_equality]){
                    l_equality=i;
                }
            }
            // отслеживание случая неограниченности задачи
            if(double.IsPositiveInfinity(delta[l_equality])){
                return new SimplexAnswer("infinite solution");
            }
            // переход от старой вершины симплекса к новой
            int l=canonic.B[l_equality];
            canonic = canonic.Pivot(l,e);
        }
        return new SimplexAnswer(canonic);
    }
    public static SimplexAnswer Initialize(Standartic standartic){
        // проверка допустимости начального решения
        int index_min_b=standartic.getMinBIndex();
        if(standartic.b[index_min_b]>=0){
            return new SimplexAnswer(new Canonic(standartic));
        }
        
        // переходим к вспомогательной задаче
        Standartic aux = standartic.getAUX();
        Canonic c_aux = new Canonic(aux);

        // выводимой переменной соответствует макс по модулю отрицательное b
        int leaving=c_aux.B[index_min_b];
        int additionalVariable=aux.getAdditionalVariableIndexAUX();
        int entering=additionalVariable;
        c_aux = c_aux.Pivot(leaving,entering);

        // основной цикл поиска решения
        double[] delta=new double[c_aux.m];
        for(;;){
            //c_aux.Print();
            int e=-1;
            for(int i=0;i<c_aux.n;i++){
                if(c_aux.c[i]>0){
                    e=i;
                    break;
                }
            }
            if(e==-1){
                break;
            }
            for(int equality=0;equality<c_aux.B.Length;equality++){
                if(c_aux.A[equality][e]<0){
                    delta[equality]=-c_aux.b[equality]/c_aux.A[equality][e];
                } else {
                    delta[equality]=double.PositiveInfinity;
                }
            }
            int l_equality=0;
            for(int i=0;i<c_aux.B.Length;i++){
                if(delta[i]<delta[l_equality]){
                    l_equality=i;
                }
            }
            int l=c_aux.B[l_equality];
            c_aux = c_aux.Pivot(l,e);
        }

        if(c_aux.getVariableNValue(additionalVariable)!=0){
            return new SimplexAnswer("no solutions");
        }
        
        // переход от вспомогательной задачи к основной
        Canonic canonic = new Canonic(standartic,c_aux);
        return new SimplexAnswer(canonic);
    }
}

class ZSolution{
    public static double functionZMax;
    public static SimplexAnswer SolveZLP(Standartic standartic){
        Canonic canonic = new Canonic(standartic);
        SimplexAnswer solution = Solution.Simplex(standartic);
        if(solution.type=="no solutions" || solution.type=="infinite solution"){
            return solution;
        }
        // конечное решение существует
        functionZMax = double.NegativeInfinity;
        return SolveZLP2(standartic);
    }
    public static SimplexAnswer SolveZLP2(Standartic task){
        Console.WriteLine("{0}{{",task.getHash());
        SimplexAnswer res = SolveZLP2_C(task);
        Console.WriteLine("}");
        return res;
    }
    public static SimplexAnswer SolveZLP2_C(Standartic task){
    // SolveZLP2 возвращает решение не хуже functionZMax, либо "denied"
        task.Print();
        SimplexAnswer task_solution = Solution.Simplex(task);
        Utils.PrintVector(task_solution.canonic.getXValue());
        Canonic task_canonic = task_solution.canonic;
        if(task_solution.type=="no solutions" || task_solution.type=="solved"
        && task_canonic.getFunctionValue()<functionZMax){
            return new SimplexAnswer("denied");
        }
        // поиск нецелочисленной компоненты решения
        int notZVariableIndex=-1;
        for(int i=0;i<task_canonic.getOriginalVariableAmount();i++){
            if(!Utils.isInt(task_canonic.getVariableNValue(i))){
                notZVariableIndex=i;
                break;
            }
        }
        if(notZVariableIndex==-1){
            // найдено целочисленное решение
            return task_solution;
        }
        double notZVariableValue = task_canonic.getVariableNValue(notZVariableIndex);
        double ceiling=Math.Ceiling(notZVariableValue);
        double floor=Math.Floor(notZVariableValue);
        Standartic task1 = new Standartic(task);
        task1.addRestriction(notZVariableIndex,"<=",floor);
        Standartic task2 = new Standartic(task);
        task2.addRestriction(notZVariableIndex,">=",ceiling);
        SimplexAnswer solution1=SolveZLP2(task1);
        SimplexAnswer solution2=SolveZLP2(task2);
        if(solution1.type=="denied"){
            return solution2;
        }
        if(solution2.type=="denied"){
            return solution1;
        }
        double z1=solution1.canonic.getFunctionValue();
        double z2=solution2.canonic.getFunctionValue();
        if(z1>=z2){
            return solution1;
        }
        return solution2;
    }
}

class Program{
    static void Help(){
        Console.WriteLine("usage:");
        Console.WriteLine("\t-sr src-file");
        Console.WriteLine("\t\treads task and shows in standard form");
        Console.WriteLine("\t-s2c src-file");
        Console.WriteLine("\t\treads task and shows in canonic form");
        Console.WriteLine("\t-p src-file leaving entering");
        Console.WriteLine("\t\treads task, applies pivot(leaving,entering) "+
            "and shows resulting canonic form");
        Console.WriteLine("\t-i src-file");
        Console.WriteLine("\t\treads task, applies initializer "+
            "and shows resulting canonic form");
        Console.WriteLine("\t-s src-file");
        Console.WriteLine("\t\treads task, solves it and shows answer");
        Console.WriteLine("\t-z src-file");
        Console.WriteLine("\t\treads task, solves it as ZLP and shows answer");
        Console.WriteLine("Format of task file:");
        Console.WriteLine("\tn # оригинальных переменных\n"+
            "\tm # уравнений\n"+
            "\tc1 .. cn\n"+
            "\t...\n"+
            "\ta{Bi}1 ... a{Bi}n <= b{Bi}\n"+
            "\t...");
    }
    static void ProcessStandarticRead(string file){
        Standartic standartic=new Standartic();
        standartic.LoadFromFile(file);
        standartic.Print();
    }
    static void ProcessStandarticToCanonic(string file){
        Standartic standartic=new Standartic();
        standartic.LoadFromFile(file);
        Canonic canonic = new Canonic(standartic);
        canonic.Print();
    }
    static void ProcessPivot(string file,int  leaving,int entering){
        Standartic standartic=new Standartic();
        standartic.LoadFromFile(file);
        Canonic canonic = new Canonic(standartic);
        if(leaving==-1){
            leaving = standartic.n; // first additional variable by default
        }
        if(entering==-1){
            entering = 0; // first original variable by default
        }
        canonic = canonic.Pivot(leaving,entering);
        canonic.Print();
    }
    static void ProcessInitializer(string file){
        Standartic standartic=new Standartic();
        standartic.LoadFromFile(file);
        SimplexAnswer simplexAnswer = Solution.Initialize(standartic);
        if(simplexAnswer.type!="solved"){
            Console.WriteLine($"{simplexAnswer.type}");
            return;
        }
        Canonic canonic = simplexAnswer.canonic;
        double[] X=canonic.getXValue();
        if(Utils.isNullVector(X)){
            Console.WriteLine("null vector");
        } else {
            Console.WriteLine("not null vector");
        }
        bool allowed=standartic.checkXAllowability(X);
        Console.WriteLine("{0}",allowed?"true":"false");
    }
    static void ProcessSimplex(string file){
        Standartic standartic=new Standartic();
        standartic.LoadFromFile(file);
        SimplexAnswer simplexAnswer = Solution.Simplex(standartic);
        if(simplexAnswer.type!="solved"){
            Console.WriteLine($"{simplexAnswer.type}");
            return;
        }
        Canonic canonic = simplexAnswer.canonic;
        double[] X=canonic.getXValue();
        Utils.PrintVector(X);
    }
    static void ProcessZLP(string file){
        Standartic standartic=new Standartic();
        standartic.LoadFromFile(file);
        SimplexAnswer simplexAnswer = ZSolution.SolveZLP(standartic);
        if(simplexAnswer.type!="solved"){
            Console.WriteLine($"{simplexAnswer.type}");
            return;
        }
        Canonic canonic = simplexAnswer.canonic;
        double[] X=canonic.getXValue();
        Utils.PrintVector(X);
    }
    static void Main(string[] args){
        if(args.Length<2){
            Help();
            return;
        }
        string srcFile=args[1];
        if(args.Length==2 && args[0]=="-sr"){
            ProcessStandarticRead(srcFile);
        } else if(args.Length==2 && args[0]=="-s2c"){
            ProcessStandarticToCanonic(srcFile);
        } else if(args.Length>=2 && args.Length<=4 && args[0]=="-p"){
            int leaving;
            int entering;
            if(args.Length>=3){
                leaving = int.Parse(args[2]);
            } else {
                leaving=-1; // default leaving
            }
            if(args.Length>=4){
                entering = int.Parse(args[3]);
            } else {
                entering = -1; // default enterin
            }
            ProcessPivot(srcFile,leaving,entering);
        } else if(args.Length==2 && args[0]=="-i"){
            ProcessInitializer(srcFile);
        } else if(args.Length==2 && args[0]=="-s"){
            ProcessSimplex(srcFile);
        } else if(args.Length==2 && args[0]=="-z"){
            ProcessZLP(srcFile);
        } else {
            Help();
        }
    }
}

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
}

class Standartic{
    public int n; // кол-во оригинальных переменных
    public int m; // кол-во неравенств-ограничений
    public double[][] A; // матрица системы неравенств
    public double[] b; // свободные члены (правая часть неравенств)
    public double[] c; // вектор коэффициентов в целевой функции
    public Standartic(){} // по умолчанию ничего не существует
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
        Console.WriteLine("Standartic");
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
        return n;
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
    public Canonic(Canonic copy){
        n=copy.n;
        m=copy.m;
        v=copy.v;
        B=new int[copy.B.Length];
        for(int i=0;i<B.Length;i++){
            B[i]=copy.B[i];
        }
        b=new double[copy.b.Length];
        for(int i=0;i<b.Length;i++){
            b[i]=copy.b[i];
        }
        c=new double[copy.c.Length];
        for(int i=0;i<c.Length;i++){
            c[i]=copy.c[i];
        }
        A=new double[copy.A.Length][];
        for(int i=0;i<A.Length;i++){
            A[i] = new double[copy.A[i].Length];
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
            for(int j=0;j<standartic.n;j++){
                A[i][j]=standartic.A[i][j];
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
    public void Print(){
        Console.WriteLine("Canonic");
        Console.Write("z={0}",v);
        for(int i=0;i<n;i++){
            Console.Write(" + ({0})*x{1}",c[i],i);
        }
        Console.WriteLine();
        for(int i=0;i<m;i++){
            Console.Write("x{0} = {1}",B[i],b[i]);
            for(int j=0;j<n;j++){
                Console.Write(" - ({0})*x{1}",A[i][j],j);
            }
            Console.WriteLine();
        }
    }
    private int equalityByBasic(int vIndex){
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
        freeMember/=coefficients[entering];
        coefficients[leaving]=1.0/coefficients[entering];
        for(int i=0;i<n;i++){
            if(i==leaving || i==entering){
                continue;
            }
            coefficients[i]/=coefficients[entering];
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
            coefficients[i]=coefficients[i]+coefficients[variable]*vCoefficients[i];
        }
        coefficients[variable]=0;
    }
    private void PivotRecalculateRestrictions(int leaving,int entering){
        int equality = equalityByBasic(leaving);
        B[equality]=entering;
        exchangeVariablesInEquation(leaving,entering,ref A[equality],
        ref b[equality]);
        for(int i=0;i<m;i++){
            if(i==equality){
                continue;
            }
            substituteVariableInEquation(entering,A[equality],b[equality],
            ref A[i],ref b[i]);
        }
    }
    private void PivotRecalculateFunction(int leaving,int entering){
        int equality = equalityByBasic(leaving);
        substituteVariableInEquation(entering,A[equality],b[equality],ref c,
        ref v);
    }
    private void PivotRedivideBasics(int leaving,int entering){
        int equality = equalityByBasic(leaving);
        B[equality]=entering;
    }
    public Canonic Pivot(int leaving,int entering){
        Canonic result=new Canonic(this);
        PivotRecalculateRestrictions(leaving,entering);
        PivotRecalculateFunction(leaving,entering);
        PivotRedivideBasics(leaving,entering);
        return result;
    }
    public double getVariableXNValue(int index){
        for(int i=0;i<m;i++){
            if(B[i]==index){
                return b[i];
            }
        }
        return 0;
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
            Console.Write("x{0}={1} ",i,canonic.getVariableXNValue(i));
        }
        Console.WriteLine();
    }
}

class Program{
    static SimplexAnswer Simplex(Standartic standartic){
        // поиск начального допустимого решения
        SimplexAnswer startingSolution=Initialize(standartic);
        if(startingSolution.type!="solved"){
            return startingSolution;
        }
        // поиск оптимального решения
        Canonic canonic=startingSolution.canonic;
        for(;;){
            canonic.Print();
            // поиск вводимой переменной
            int e=-1;
            for(int i=0;i<canonic.n;i++){
                if(canonic.c[i]>0){
                    e=i;
                    break;
                }
            }
            // если все коэф-ты в z <=0 достигнуто оптимальное решение
            if(e==-1){
                break;
            }
            // поиск максимального приращения вводимой переменной
            double[] delta=new double[canonic.m];
            for(int equality=0;equality<canonic.B.Length;equality++){
                if(canonic.A[equality][e]>0){
                    delta[equality]=canonic.b[equality]/canonic.A[equality][e];
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
                return new SimplexAnswer("Задача неограничена");
            }
            // переход от старой вершины симплекса к новой
            int l=canonic.B[l_equality];
            canonic = canonic.Pivot(l,e);
        }
        return new SimplexAnswer(canonic);
    }
    static SimplexAnswer Initialize(Standartic standartic){
        int index_min_b=standartic.getMinBIndex();
        Console.WriteLine("index_min_b={0}",index_min_b);
        // пустимо ли начальное решение
        if(standartic.b[index_min_b]>=0){
            Console.WriteLine("Start sol ok");
            return new Canonic(standartic);
        }
        
        // переходим к вспомогательной задаче
        Standartic aux = standartic.getAUX();
        Canonic c_aux = new Canonic(aux);

        // выводимой переменной соответствует макс по модулю отрицательное b
        int leaving=c_aux.B[index_min_b];
        int entering=aux.getAdditionalVariableIndexAUX();
        c_aux = c_aux.Pivot(leaving,entering);


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
                if(c_aux.A[equality][e]>0){
                    delta[equality]=c_aux.b[equality]/c_aux.A[equality][e];
                } else {
                    delta[equality]=double.PositiveInfinity;
                }
            }
            /*Console.Write("delta: ");
            for(int equality=0;equality<delta.Length;equality++){
                Console.Write("{0}/{1}={2} ",c_aux.b[equality],
                    c_aux.A[equality][e],delta[equality]);
            }
            Console.WriteLine();//*/
            int l_equality=0;
            for(int i=1;i<c_aux.B.Length;i++){
                if(delta[i]<delta[l_equality]){
                    l_equality=i;
                }
            }
            int l=c_aux.B[l_equality];
            //Console.WriteLine("e={0} l={1}",e,l);
            c_aux = c_aux.Pivot(l,e);
        }
        if(c_aux.getVariableXNValue(aux.n)!=0){
            Console.WriteLine("Задача неразрешима");
        } else {
            if(Utils.DoesArrayConsistsElement(c_aux.B,aux.n)){
                
            }
        }
        return new Canonic();
    }
    static void Main(string[] args)
    {
        //Canonic canonic = new Canonic();
        //canonic.LoadFromFile("simpInf.txt");
        //Simplex(canonic);
        Standartic standartic = new Standartic();
        standartic.LoadFromFile("stan1.txt");
        standartic.Print();
        Standartic aux;
        aux = standartic.getAUX();
        aux.Print();
        //Canonic canonic = new Canonic();
        //canonic = Initialize(standartic);
    }
}

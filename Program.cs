﻿using System;
using System.IO;

class Canonic{
    public int n;
    public int m;
    public int[] N;
    public int[] B;
    public double[][] A;
    public double[] b;
    public double[] c;
    public double v;
    public Canonic(){}
    public Canonic(Canonic copy){
        n=copy.n;
        m=copy.m;
        v=copy.v;
        N=new int[copy.N.Length];
        for(int i=0;i<N.Length;i++){
            N[i]=copy.N[i];
        }
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
    bool consists(int[] arr,int element){
        for(int i=0;i<arr.Length;i++){
            if(arr[i]==element){
                return true;
            }
        }
        return false;
    }
    public void LoadFromFile(string path){
        StreamReader R;
        R = new StreamReader(path);
        string str;
        string[] strArr;
        // n
        n = int.Parse(R.ReadLine());
        // m
        m = int.Parse(R.ReadLine());
        // v
        v = double.Parse(R.ReadLine());
        // c
        str = R.ReadLine();
        strArr = str.Split(" ");
        c = new double[n];
        for(int i=0;i<n;i++){
            c[i]=double.Parse(strArr[i]);
        }
        // B
        str = R.ReadLine();
        strArr = str.Split(" ");
        B = new int[m];
        for(int i=0;i<m;i++){
            B[i]=int.Parse(strArr[i]);
        }
        // N
        N = new int [n];
        int index=0;
        for(int i=0;index<n;i++){
            if(!consists(B,i)){
                N[index]=i;
                index++;
            }
        }
        // A & b
        A = new double[m][];
        for(int i=0;i<m;i++){
            A[i]=new double[n];
        }
        b = new double[m];
        for(int i=0;i<m;i++){
            str = R.ReadLine();
            strArr = str.Split(" ");
            b[i] = double.Parse(strArr[0]);
            for(int j=0;j<n;j++){
                A[i][j]=double.Parse(strArr[1+j]);
            }
        }
        R.Close();
    }
    public void Print(){
        Console.WriteLine("Canonic");
        Console.Write("z={0}",v);
        for(int i=0;i<n;i++){
            Console.Write(" + {0}*x{1}",c[i],i);
        }
        Console.WriteLine();
        for(int i=0;i<m;i++){
            Console.Write("x{0} = {1}",B[i],b[i]);
            for(int j=0;j<n;j++){
                Console.Write(" - {0}*x{1}",A[i][j],j);
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
    public Canonic Pivot(int l,int e){
        Canonic result=new Canonic(this);
        int equality = equalityByBasic(l);
        result.B[equality]=e;
        result.c[e]=0;
        for(int i=0;i<m;i++){
            result.A[i][e]=0;
        }
        result.b[equality]=b[equality]/A[equality][e];
        result.A[equality][l]=1/A[equality][e];
        for(int i=0;i<n;i++){
            if(i==e||i==l){
                continue;
            }
            result.A[equality][i]=A[equality][i]/A[equality][e];
        }
        for(int k=0;k<m;k++){
            if(k==equality){
                continue;
            }
            result.b[k]=b[k]-A[k][e]*result.b[equality];
            for(int i=0;i<n;i++){
                if(i==e){
                    continue;
                }
                result.A[k][i]=A[k][i]-A[k][e]*result.A[equality][i];
            }
        }
        result.v=v+c[e]*result.b[equality];
        for(int i=0;i<n;i++){
            if(i==e){
                continue;
            }
            result.c[i]=c[i]-c[e]*result.A[equality][i];
        }
        return result;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Canonic canonic = new Canonic();
        canonic.LoadFromFile("2.txt");
        canonic.Print();
        //canonic = canonic.Pivot(3,0);
        canonic = canonic.Pivot(1,0);
        canonic.Print();
    }
}

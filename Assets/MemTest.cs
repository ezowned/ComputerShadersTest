using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//прибавим ко всем элементам массива A единицы в вычислительном шейдере
//и сравним производительность шейдеров
public class MemTest : MonoBehaviour {

    int[] A = new int[4096];
    const int N = 100000; int u=0;
    ComputeBuffer bomputerCuffer;
    public ComputeShader shader;

    private void Awake()
    {
        var tm = Time.realtimeSinceStartup;
        for (int i = 0; i < N*100; i++) u++;
        tm = Time.realtimeSinceStartup - tm;
        Debug.Log("On CPU time: " + tm * A.Length);
    }

    void Start()
    {

        TestKernel(shader.FindKernel("F0"), "Global memory test:   ");
        TestKernel(shader.FindKernel("F1"), "Shared memory test:  ");
        TestKernel(shader.FindKernel("F2"), "Register memory test: ");
        TestKernel(shader.FindKernel("F3"), "Empty kernel test:        ");
    }

    public void TestKernel(int kernelid, string msg)
    {
        for (int i = 0; i < A.Length; i++) A[i] = 0;
        bomputerCuffer = new ComputeBuffer(A.Length, sizeof(int));
        shader.SetBuffer(kernelid, "data", bomputerCuffer);
        bomputerCuffer.SetData(A);

        var tm = Time.realtimeSinceStartup;
        for (int i = 0; i < 100; i++)
            shader.Dispatch(kernelid, 64, 1, 1);
        bomputerCuffer.GetData(A);
        tm = Time.realtimeSinceStartup - tm;

        bomputerCuffer.Release();
        bomputerCuffer.Dispose();
        bomputerCuffer = null;

        for (int i = 0; i < A.Length; i++)
            if (A[i] != u)
            {
                Debug.Log(msg + "[FALS]" + ", time: " + tm);
                return;
            }
        Debug.Log(msg + "[TRUE]" + ", time: " + tm);
    }





}

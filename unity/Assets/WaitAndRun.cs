using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WaitAndRun : MonoBehaviour
{
    public Text checkFinishedText;
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        
        const int iterations = 10;
        using (var file = new StreamWriter("results.txt"))
        {
            for (int i = 0; i < iterations; i++)
            {
                var result = RefsBenchmark.main();
                file.WriteLine(result);
            }
        }

        checkFinishedText.text = "DONE!";
    }
}

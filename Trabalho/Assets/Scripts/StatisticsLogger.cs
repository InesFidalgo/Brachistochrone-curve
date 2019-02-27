using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StatisticsLogger {
	
	public Dictionary<int,float> bestFitness;
	public Dictionary<int,float> meanFitness;
    public Dictionary<int, float> desviopadrao;
    public Dictionary<int, float> worstFitness;
    

    private string filename;
	private StreamWriter logger;


	public StatisticsLogger(string name) {
		filename = name;
		bestFitness = new Dictionary<int,float> ();
		meanFitness = new Dictionary<int,float> ();
        desviopadrao = new Dictionary<int, float>();
        worstFitness = new Dictionary<int, float>();

    }

    //saves fitness info and writes to console
    public void PostGenLog(List<Individual> pop, int currentGen) {
        
        int tam_pop = pop.Count - 1;

        pop.Sort((x, y) => x.fitness.CompareTo(y.fitness));

        bestFitness.Add(currentGen, pop[0].fitness);
        meanFitness.Add(currentGen, 0f);
        desviopadrao.Add(currentGen, 0f);
		worstFitness.Add(currentGen, pop[tam_pop].fitness);

        foreach (Individual ind in pop) {
            meanFitness[currentGen] += ind.fitness;
        }
        meanFitness[currentGen] /= pop.Count;


        ///////////////////////////////////////////devio padrao
        List<float> valores = new List<float>();
        foreach (Individual ind in pop)
        {
            valores.Add(ind.fitness); 
        }
        
		List<float> valores_dp = new List<float>();
        foreach (float valor in valores)
        {
            valores_dp.Add((valor - meanFitness[currentGen]) * (valor - meanFitness[currentGen]));
        }
        float sum_valoresdp = 0;
        for(int i = 0; i < valores_dp.Count; i++)
        {
            sum_valoresdp += valores_dp[i];
        }

        desviopadrao[currentGen] = (float) System.Math.Sqrt(sum_valoresdp / (valores_dp.Count));
        ///////////////////////////////////////////////

		Debug.Log ("generation: "+currentGen+"\tbest: " + bestFitness [currentGen] + "\tmean: " + meanFitness [currentGen]+ "\tdesvio padrao: " + desviopadrao[currentGen] + "\tworst: " + worstFitness[currentGen] + "\n");
	}

	//writes to file
	public void finalLog() {
		logger = File.CreateText (filename);

		//writes with the following format: generation, bestfitness, meanfitness
		for (int i=0; i<bestFitness.Count; i++) {
            logger.Write(bestFitness[i] + "\t");
            //logger.Write(meanFitness[i] +"\t");
            //logger.Write(desviopadrao[i] + "\t");
            //logger.Write(worstFitness[i]);
            logger.Write("\n");

        }
		logger.Close ();
	}

    
}

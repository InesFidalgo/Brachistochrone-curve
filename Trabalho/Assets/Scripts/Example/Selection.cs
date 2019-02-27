using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selection : SelectionMethod
{

	public Selection() : base()
	{

	}


	public override List<Individual> selectIndividuals(List<Individual> oldpop, int num, float k)
	{
       
		return selection(oldpop, num, k);
	}


	List<Individual> selection (List<Individual> oldpop, int num, float k)
	{
		List<Individual> aux = new List<Individual>();
		List<Individual> selectedInds = new List<Individual>();
		int popsize = oldpop.Count;
		if (num > 1)
		{
            

			//torneio
			for (int j = 0; j < popsize; j++)
			{
				Individual ind = null;
				float r = Random.value;
				for (int i = 0; i < num; i++)
				{
                    int rand = Random.Range(0, popsize);
                    while (selectedInds.Contains(ind))
                    {
                        rand = Random.Range(0, popsize);
                    }
					aux.Add(oldpop[rand]);

				}

                aux.Sort((x, y) => x.fitness.CompareTo(y.fitness));
                if (r < k)
				{
					ind = aux[0];
				}
				else
				{
					float pior = 0f;
					for(int l=0;l< num; l++)
					{
						if (aux[l].fitness > pior)
						{
							pior = aux[l].fitness;
							ind = aux[l];
						}
					}
				}

				selectedInds.Add(ind.Clone());
				aux.Clear();

			}
			

		}
		else if (num == 1)
		{
			{
				//roleta
				List<double> roleta = new List<double>();
				float total = 0;
				for (int i = 0; i < popsize; i++)
				{
					roleta[i] = 0;
				}
				for (int i = 0; i < popsize; i++)
				{
					total += (float) 1.0 / oldpop[i].fitness;
					roleta[i] = total;
				}
				float res = Random.Range(0, total);
				for (int i = 0; i < popsize; i++)
				{
					if (roleta[i] > res)
					{

						Individual ind = oldpop[i];
                        if (!selectedInds.Contains(ind))
                        {
                            selectedInds.Add(ind.Clone());
                        }
						
					}
				}


			}

			
		}
       
        return selectedInds;

    }
}
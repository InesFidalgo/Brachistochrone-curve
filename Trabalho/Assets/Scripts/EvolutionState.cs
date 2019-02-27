using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvolutionState : MonoBehaviour {

    public int currTest;
    public int testTimes;
    public float startPointX;
    public float startPointY;
    public float endPointX;
    public float endPointY;
    public float g;
    public float startVelocity;
    public int numTrackPoints;
    private ProblemInfo info;
    public bool elitismo;
    public float quantidadeEli;
    public int numGenerations;
    public int populationSize;
    public bool mutationGauss;
    public float mutationProbability;
    public bool halfCross;
    public float crossoverProbability;
    public bool randomS;
    public int ncortes;
    public int tamtorneio;
    public float k;

    private List<Individual> population;
    private SelectionMethod randomSelection;
    private SelectionMethod selection;

    private int evaluatedIndividuals;
    private int currentGeneration;
    public int EvaluationsPerStep;

    private StatisticsLogger stats;
    public string statsFilename;

    private PolygonGenerator drawer;


    bool evolving;
    bool drawing;

	// Use this for initialization
   

	void Start() {
       
        info = new ProblemInfo();
        info.startPointX = startPointX;
        info.startPointY = startPointY;
        info.endPointX = endPointX;
        info.endPointY = endPointY;
        info.g = g;
        info.startVelocity = startVelocity;
        info.numTrackPoints = numTrackPoints;
        info.ncortes = ncortes;

        randomSelection = new RandomSelection(); //change accordingly
        selection = new Selection();
        stats = new StatisticsLogger(statsFilename+currTest);

        drawer = new PolygonGenerator();

        InitPopulation();
        evaluatedIndividuals = 0;
        currentGeneration = 0;
        evolving = true;
        drawing = false;
    }



    void FixedUpdate () {
		if (evolving) {
			EvolStep ();
		} else if(drawing) {
            for(int i = 0; i < population.Count; i++)
            {
                population[i].evaluate();
            }
			population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
			drawer.drawCurve(population[0].trackPoints,info);
			drawing=false;
            if (currTest != testTimes - 1)
            {
                currTest++;
                Start();
            }
        }
        
        
	}


	void EvolStep() {
		//do for a given number of generations
		if (currentGeneration < numGenerations) {
			
			//if there are individuals to evaluate on the current generation
			int evalsThisStep = EvaluationsPerStep < (populationSize - evaluatedIndividuals) ? EvaluationsPerStep : (populationSize - evaluatedIndividuals);
			for (int ind = evaluatedIndividuals; ind<evaluatedIndividuals+evalsThisStep; ind++) {
				population[ind].evaluate();
			}
			evaluatedIndividuals += evalsThisStep;
			
			//if all individuals have been evaluated on the current generation, breed a new population
			if(evaluatedIndividuals==populationSize) {
				stats.PostGenLog(population,currentGeneration);
				
				population = BreedPopulation();
				evaluatedIndividuals=0;
				currentGeneration++;
			}
			
		} else {
			stats.finalLog();
			evolving=false;
			drawing = true;
			print ("evolution stopped");
		}
		
		

	
	}


	void InitPopulation () {
		population = new List<Individual>();
		while (population.Count<populationSize) {
			ExampleIndividual newind = new ExampleIndividual(info); //change accordingly
			newind.Initialize();
			population.Add (newind);
		}
	}


	List<Individual> BreedPopulation() {
		List<Individual> newpop = new List<Individual>();
        List<Individual> selectedInds;
        //breed individuals and place them on new population. We'll apply crossover and mutation later 
        

        while (newpop.Count<populationSize) {
            if (randomS)
            {
                selectedInds = randomSelection.selectIndividuals(population, tamtorneio, k);
            }
            else
            {
                selectedInds = selection.selectIndividuals(population, tamtorneio, k);
            }

            //we should propably always select pairs of individuals

            //  //we should propably always select pairs of individuals

            if (elitismo)
            {
                population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
                for (int i = 0; i < (int)(quantidadeEli* populationSize); i++)
                {
                    newpop.Add(population[i]);
                }

                for (int i = (int)(quantidadeEli * populationSize); i < selectedInds.Count; i++)
                {
                    if (newpop.Count < (populationSize - quantidadeEli))
                    {
                       
                        newpop.Add(selectedInds[i]); //added individuals are already copys, so we can apply crossover and mutation directly
                    }
                    else { //discard any excess individuals
                        selectedInds.RemoveAt(i);
                    }

                }
            }

            else {
                for (int i = 0; i < selectedInds.Count; i++)
                {
                    if (newpop.Count < populationSize)
                    {
                        newpop.Add(selectedInds[i]); //added individuals are already copys, so we can apply crossover and mutation directly
                    }
                    else { //discard any excess individuals
                        selectedInds.RemoveAt(i);
                    }
                }
            }
            

			//apply crossover between pairs of individuals and mutation to each one
			while(selectedInds.Count>1) {
				selectedInds[0].Crossover(selectedInds[1],crossoverProbability);
				selectedInds[0].Mutate(mutationProbability);
				selectedInds[1].Mutate(mutationProbability);
				selectedInds.RemoveRange(0,2);
			}
			if(selectedInds.Count==1) {
				selectedInds[0].Mutate(mutationProbability);
				selectedInds.RemoveAt(0);
			}
		}

		return newpop;
	}

}



# STAARC: STochastic Atom-based ARtificial Chemistry

This repository contains the files necessary to simulate Hutton Artificial Chemistry (HuAC) using Simulation Stochastic Algorithm (SSA) also known as Gillespie Algorithm


## The code 

This program is in C# and has been build using Xamarin. 
Dependencies:
- QuickGraph 
- Newton Json 
both NuGet packages. 


## Quick start 

To start simply add 'greactor` as a reference and the using command 

```
using greactor; 
```

To create a reactor simply create a new instance:
```
var rec =  new reactor();
```

Now you can add your reactions using 'add_rule` method. This method takes a rule object that needs to be created. It is a base class you never instantiate. You need to use the derived class

- b_rule : for bimolecular reactions 
- d_rule : for degradation reactions 
- o_rule : for production reactions
- m_rule : for unimolecular modifications 



Bimolecular reactions: for the moment, the main API is a pain :) but here are examples:

for (t1,s1) + (t2,s2) -> (t1, s3).(t2,s4) at rate r 
```
var r = new b_rule(false,t1,s1,t2,s2,true, t1,s3,t2,s4,idx, r);
```

for (t1,s1).(t2,s2) -> (t1, s3)+(t2,s4) at rate r 
```
var r = new b_rule(true,t1,s1,t2,s2,false, t1,s3,t2,s4,idx, r);
```

Then you can add particles using 'add_particles` and the struct particle. To add a particle (t1,s1)
```
rec.add_particle(new particle (t1, s1));
```

Then you can run the Gillespie algorithm 

```
int seed = 1;
Random rd = new Random (seed);
int step = 0;
int idx;
double tau, total_time = 0.0;

while (rec.gillespie_step (rd, out tau, out idx)) {
      total_time +=tau;
      step++;
}
```

The method gillespie_step takes a random generator and exit the time to the next reaction and the index of the method that has been applied. It returns true if there is a reaction actually taking place. If the propensities are all zero gillespie_step returns false. 

### notes:

1- The idx returned here is internal you can get the method using 

```
rule r = rec.rec.get_rule(idx) 
``
2- The total_time and step computation are optional it is only to keep track of time and reactions 

## Versions:

2016/02/26:  version is 1.0.0 and should be working 


## TODOS: 
- insert graph command into reactor's methods
- more test functions 
- finish trimolecular (enzyme) reaction 



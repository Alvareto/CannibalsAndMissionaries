# Cannibals And Missionaries
College project (Advanced Operating Systems) - C# implementation of process synchronization algorithms (2017/2018)

# Information
Process communication is done using **message queues**.

Process synchronization is done using a **Token Ring Algorithm**.

# Setup
At the shore of some wide river there is a boat that transfers cannibals and missionaries to the other side of the river. Boat capacity is 3 passengers and it has to have at least 1 passenger in order to go. Inside the boat there can't be more cannibals than missionaries, but all other combinations are allowed. 

At the start the main process, which represents the boat, creates N cannibal processes and M missionary processes (N>1 and M>1 are inputs). 
All missionaries, cannibals and boat are on the same side of the river at the beginning. Cannibals and missionaries want to use boat to cross to the other side of the river. After they cross the river, passengers leave (there are no longer in system, *ergo* cannibal or missionary process is terminated), and boat returns to the origin side of the river. Boat waits 4 seconds for passengers to board and goes if there is at least 1 passenger inside. If no one boarded for 4 seconds, the main boat process is terminated.

Boat process writes down all passengers on every transfer (for example: "Transfered: missionary, missionary, cannibal").
Procceses are communicating with each other using message queues and are correctly synchronized using token ring algorithm.
All processes write down message they send and message they receive.

# Far From Here

## 📌 Overview
Far From Here is a 3D survival game with horror elements. The player is lost in a forest where they need to survive and attempt to make their way back to civilization.

## 🕹️ Core Gameplay
Player survives by scavenging the forest for food and water.
Player needs to avoid or eliminate threats like animals and other cryptid or eldritch entities.
The player can find hunting rifles and ammo in abandoned cabins and watchtowers to do the above.
The goal of the player is to reach an extraction point, they learn the location of through items like a radio, GPS and compass.
The radio gives coordinates and a direction from those coordinates, the gps tells the player what their coordinates are and the compass gives the direction of the player
They lose when their hit points reach 0, through starvation, dehydration or enemy entities attacking the player.

## 🎯 Game Type
Survival and Horror

## 👥 Player Setup
Single player as the survivor
Optional co-op where 1-3 additional player(s) can join the same world and attempt to reach the extraction together

## 🤖 AI Design
### Enemy Entity FSM
Roaming, walks around aimlessly
Alerted through sound, tracks, or previously sighted player
Chasing player
Attacking player
Searching for previously sighted player
Fleeing, animals will flee at low hp (not including cryptid or eldritch entities)

## 🎬 Scripted Events
When player gets all 3 items (radio, GPS and compass) spawns extraction point in the world
When the player picks up the radio (or clicks on it again) it will play a recording with a coordinate and a direction from that coordinate
Quick time event when an enemy attacks a player, they will continously take damage during the quick time event
Player succesfuly makes it to extraction plays win cutscene

## 🌍 Environment
As large as a forest as can be reasonably made and filled
NavMesh baked for AI pathing  
trees, rocks and abandoned structures have colliders

## 🧪 Physics Scope
Rigidbody on player(s) and enemies
Colliders and triggers on environment and entities
Raycast for player shooting firearm
Physics materials for items being dropped (food, rations, weapons and objective items)
Layer-based collision matrix hit and hurt boxes, world static, interaction range
Simple ragdoll or flail on death of player or enemy

## 🧠 FSM Scope
State machines implemented for player and enemies
Event driven transitions using Unity events and C# events    
Debug overlay that prints current states for grading

## 🧩 Systems and Mechanics
Player survives by keeping their hit points above 0, which requires consuming food and water as well as avoiding danger
Player can obtain a firearm and a knife to protect themselves, eliminating enemies from range or having easier quick time events at close range
Player wins by extracting after collecting all Objective items that point towards an extraction point

Object tagging Weapon, Consumable, Objective
Camera first person 
Audio cues of danger nearby with intense "battle" music or enemy sounds
VFX blood when the player takes damage, effects on the horror elemnts time permitting

## 🎮 Controls (proposed)
W A S D move  
Shift sprint
Mouse look  
Space jump  
F interact pick up, open and close door
Left Mouse shoot
Right Mouse aim down sights
Esc pause

## 📂 Project Setup aligned to course topics
Unity 6.0 (6000.0.58f1 LTS)
C# scripts for PlayerController, EnemyAIController, Consumables, ObjectiveItems, InventoryController, ExtractionController
NavMesh for AI pathing  
Animator controllers for player and enemies with parameters speed, isAttacking, isWalking, isRunning 
Physics materials and layers configured in Project Settings  
GitHub Classroom repository with regular commits and meaningful messages  
Readme and in game debug UI showing FPS, state names, and safety meter for assessment
import csv
import json
import matplotlib
import matplotlib.pyplot as plt
import numpy as np

answers = [[], []]

def fetchAnswers():
    for type in [1, 3]:
        for i in range(20):
            with open('results/data/2D_T{0}_{1}.json'.format(type, i)) as data:
                text = data.read()

                iOne = text.find("marked")
                iTwo = text.find("marked", iOne + 1)

                dOne = float(text[iOne + 19 : iOne + 21])
                dTwo = float(text[iTwo + 19 : iTwo + 21])


                if (dOne > dTwo):
                    correct = 0
                    ratio = dTwo / dOne
                else:
                    correct = 1
                    ratio = dOne / dTwo

                answers[type==3] += [[correct, ratio]]






def main():
    indivRec = []
    indivResults = []

    curID = ""
    totalTimeOne = 0
    totalTimeTwo = 0
    totalScore = 0
    totalRatioErr = 0



    fetchAnswers()

    with open ('2D_Pilot.csv', 'r') as csvfile:
        reader = csv.reader(csvfile, delimiter = ',', quotechar='|')
        for row in reader:
            if (row[0] != curID):
                if (curID != ""):
                    #GET AVG. DISTANCE MOVED
                    #with open("Pose_" + curID + ".csv" as distFile):
                        #distReader = csv.reader(distFile, delimiter = ',', quotechar = "|")
                        #for distRow in distReader:

                    indivRec += [[curID, totalTimeOne / 60, totalTimeTwo / 60, totalScore / 60, totalRatioErr / 60]]
                curID = row[0]

                totalTimeOne = 0
                totalTimeTwo = 0
                totalScore = 0
                totalRatioErr = 0
                    #END RECORDING OF INDIVIDUAL HERE


            type = int(row[3][4])
            index = int(row[3].split('_')[2])

            totalTimeOne += float(row[4])
            totalTimeTwo += float(row[5])
            if type == 1:
                typeIndex = 0
            else:
                typeIndex = 3
            if (int(row[6]) == answers[type == 3][index][0]):
                totalScore += 1

            totalRatioErr += abs(float(row[7]) - answers[type == 3][index][1])

            rotXBool = abs(float(row[11])) > 0.1
            rotYBool = abs(float(row[12])) > 0.1

            xNorm = abs(float(row[8])) == 0.52 or abs(float(row[8])) == 0.343
            xPos = abs(float(row[8])) == 0.157 or abs(float(row[8])) == 0.02
            xNeg = not (xNorm or xPos)

            yNorm = abs(float(row[9])) == 0.74 or abs(float(row[9])) == 0.837
            yPos = not yNorm

            zNorm = (abs(float(row[10])) == 1.09 and float(row[18]) != 0) or abs(float(row[10])) == 0.391
            zPos = abs(float(row[10])) == 1.59 or (abs(float(row[10])) == 0.109 and row[18] == 0)
            zNeg = not (zNorm or zPos)

            indivResults += [[xNorm, xPos, xNeg, yNorm, yPos, 0, zNorm, zPos, zNeg, rotXBool, rotYBool, float(row[4]), float(row[5]), int(row[6]) == answers[type == 3][index][0], abs(float(row[7]) - answers[type == 3][index][1])]]

    with open('finalIndivData.csv', 'w') as f:
        writer = csv.writer(f)
        writer.writerows(indivResults)


    # with open('finalData.csv', 'w') as f:
    #     writer = csv.writer(f)
    #     writer.writerows(indivRec)


def genPlot():
    dict = {}
    with open('2D_Pilot.csv', 'r') as f:
        reader = csv.reader(f)
        for row in reader:
            type = int(row[3][4])
            index = int(row[3].split('_')[2])

            temp = answers[type == 3][index][1]

            if temp not in dict:
                dict[temp] = []

            dict[temp].append([type, answers[type == 3][index], abs(float(row[7]) - answers[type == 3][index][1])])

    truePercOne = []
    truePercThree = []

    meanErrorOne = []
    meanErrorThree = []

    for item in dict:

        x1,x3 = [],[]
        for x in dict[item]:
            if x[0] == 1:
                x1.append(float(x[2]))
            elif x[0] ==3:
                x3.append(float(x[2]))
        print('x1 length:',len(x1))
        print('x3 length:',len(x3))

        if len(x1) > 0:
            truePercOne.append(item)
            meanErrorOne.append(np.array([100.0*float(x[2]) for x in dict[item] if x[0]==1]).mean())
        if len(x3) > 0:
            truePercThree.append(item)
            meanErrorThree.append(np.array([100.0*float(x[2]) for x in dict[item] if x[0]==3]).mean())

    # print(truePerc)
    # print(meanError)

    truePercOne = np.array(truePercOne)
    truePercThree = np.array(truePercThree)
    meanErrorOne = np.array(meanErrorOne)
    meanErrorThree = np.array(meanErrorThree)


    one_inds = np.argsort(truePercOne)
    truePercOne = truePercOne[one_inds]
    meanErrorOne = meanErrorOne[one_inds]
    three_inds = np.argsort(truePercThree)
    truePercThree = truePercThree[three_inds]
    meanErrorThree = meanErrorThree[three_inds]
    print(truePercOne)
    print(meanErrorOne)
    print(truePercThree)
    print(meanErrorThree)
    f, (ax1, ax2) = plt.subplots(1, 2, sharey=True)
    ax1.plot(truePercOne, np.log2(meanErrorOne+1/8.0))
    ax2.plot(truePercThree, np.log2(meanErrorThree+1/8.0))

    plt.show()

    # print(dict)




main()
genPlot()

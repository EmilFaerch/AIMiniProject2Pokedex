# import the necessary packages
from keras.preprocessing.image import img_to_array
from keras.models import load_model
import numpy as np
import argparse
import imutils
import pickle
import cv2
import os
import socket

imageDim = 96
curPath = os.path.dirname(os.path.realpath(__file__)) + "/"

def classifyPokemon(Pokemon):
    pokePicPath = imagesPath + Pokemon +".PNG"

    print(pokePicPath)

    # load the image
    image = cv2.imread(pokePicPath)

    # pre-process the image for classification
    image = cv2.resize(image, (imageDim, imageDim))
    image = image.astype("float") / 255.0
    image = img_to_array(image)
    image = np.expand_dims(image, axis=0)

    # classify the input image
    print("[PokéDex] classifying image ...")
    proba = model.predict(image)[0]
    idx = np.argmax(proba)
    label = lb.classes_[idx]

    name = label

    # build the label and draw the label on the image
    label = "{} {:.2f}%".format(label, proba[idx] * 100)
    print(label)

    return name

host = '127.0.0.1' 
port = 5222
backlog = 5 
size = 1024 
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
s.bind((host,port)) 
s.listen(backlog) 

firstMSG = True
imagesPath = ""

# load the trained convolutional neural network and the label binarizer
model = load_model(curPath + "pokedex.model")
lb = pickle.loads(open(curPath + "lb.pickle", "rb").read())

print("\n\n\n[PokéDex] Initialized!\n[SERVER] Ready for Unity connection.")

while 1:
    client, address = s.accept() 
    print ("Unity connected.")

    while 1:
        data = client.recv(size)
        data = data.decode()
        if (data):
            if (firstMSG):
                imagesPath = data
                firstMSG = False
            else:
                reply = bytearray(classifyPokemon(data), encoding="utf-8")
                client.send(reply)


            
        data = None
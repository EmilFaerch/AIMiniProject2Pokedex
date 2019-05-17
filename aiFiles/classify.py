# import the necessary packages
from keras.preprocessing.image import img_to_array
from keras.models import load_model
import numpy as np
import argparse
import imutils
import pickle
import cv2
import os

imageDim = 96

# construct the argument parse and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-m", "--model", type=str, default="pokedex.model", required=False,
	help="path to trained model model")
ap.add_argument("-l", "--labelbin", type=str, default="lb.pickle", required=False,
	help="path to label binarizer")
ap.add_argument("-i", "--image", required=True,
	help="path to input image")
args = vars(ap.parse_args())

# load the image
image = cv2.imread(args["image"])
output = image.copy()
 
# pre-process the image for classification
image = cv2.resize(image, (imageDim, imageDim))
image = image.astype("float") / 255.0
image = img_to_array(image)
image = np.expand_dims(image, axis=0)

# load the trained convolutional neural network and the label
# binarizer
print("[INFO] loading network...")
model = load_model(args["model"])
lb = pickle.loads(open(args["labelbin"], "rb").read())
 
# classify the input image
print("[INFO] classifying image...")
proba = model.predict(image)[0]
print(proba)
idx = np.argmax(proba)
print(idx)
label = lb.classes_[idx]
print(label)



# mark 'correct' if input image contains label, e.g. "examples/pikachu.jpg" contains "pikachu"
filename = args["image"][args["image"].rfind(os.path.sep) + 10 :-4] # '+ 10' removes 'examples/' & ':-4' removes '.jpg'
correct = "correct" if filename.rfind(label) != -1 else "incorrect"
 
# build the label and draw the label on the image
label = "{}: {:.2f}% ({}, File Name: {})".format(label, proba[idx] * 100, correct, filename)
output = imutils.resize(output, width=400)
cv2.putText(output, label, (10, 25),  cv2.FONT_HERSHEY_SIMPLEX,
	0.7, (0, 255, 0), 2)
 
# show the output image
print("[INFO] {}".format(label))
cv2.imshow("Output", output)
cv2.waitKey(0)
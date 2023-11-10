import numpy as np
import pandas as pd
from pathlib import Path
from sklearn.model_selection import cross_val_score, train_test_split
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler, MinMaxScaler
from sklearn.metrics import accuracy_score, confusion_matrix, classification_report
from sklearn.svm import LinearSVC, SVC
from sklearn.calibration import CalibratedClassifierCV

import pickle

def print_score(clf, X_train, y_train, X_test, y_test, train=True):
    if train:
        pred = clf.predict(X_train)
        clf_report = pd.DataFrame(classification_report(y_train, pred, output_dict=True))
        print("Train Result:\n================================================")
        print(f"Accuracy Score: {accuracy_score(y_train, pred) * 100:.2f}%")
        print("_______________________________________________")
        print(f"CLASSIFICATION REPORT:\n{clf_report}")
        print("_______________________________________________")
        print(f"Confusion Matrix: \n {confusion_matrix(y_train, pred)}\n")
        
    elif train==False:
        pred = clf.predict(X_test)
        clf_report = pd.DataFrame(classification_report(y_test, pred, output_dict=True))
        print("Test Result:\n================================================")        
        print(f"Accuracy Score: {accuracy_score(y_test, pred) * 100:.2f}%")
        print("_______________________________________________")
        print(f"CLASSIFICATION REPORT:\n{clf_report}")
        print("_______________________________________________")
        print(f"Confusion Matrix: \n {confusion_matrix(y_test, pred)}\n")

def fit_SVM(kernels, X_train, y_train, X_test, y_test, verbose=False, save=True):
    for kernel in kernels:
        match kernel:
            case 'liner':
                model = CalibratedClassifierCV(LinearSVC(loss='hinge', dual=True), n_jobs=5)
                model.fit(X_train, y_train)       
            case 'poly':
                model = SVC(kernel='poly', degree=2, gamma='auto', coef0=1, C=5, probability=True)
                model.fit(X_train, y_train)
            case 'rbf':
                model = SVC(kernel='rbf', gamma=0.5, C=0.1, probability=True)
                model.fit(X_train, y_train)
        if verbose:     
            print_score(model, X_train, y_train, X_test, y_test, train=True)
            print_score(model, X_train, y_train, X_test, y_test, train=False)
        if save:
            filename = f'{kernel}.model'
            pickle.dump(model, open(filename, 'wb'))

label_encoding={
    'Fist':0,
    'TurnLeft':1,
    'TurnRight':2,
    'Forwards':3,
    'Backwards':4,
    'GoLeft':5,
    'GoRight':6,
}

data_folder = Path(__file__).parent.joinpath("data")
recordings = data_folder.glob("*.txt")
files = [file.__str__() for file in recordings]

all_joint_values = []
all_labels = []

for file in files:
    joint_values = np.genfromtxt(file, delimiter=",")
    label = file.split('_')[-1].split('.')[0]
    label = label_encoding[label]
    all_joint_values.append(joint_values)
    all_labels.append([label] * len(joint_values))

all_joint_values = np.concatenate(all_joint_values)
all_labels = np.concatenate(all_labels)

pipeline = Pipeline([
    ('min_max_scaler', MinMaxScaler()),
    ('std_scaler', StandardScaler())
])

X_train,X_test,y_train,y_test = train_test_split(all_joint_values,all_labels,test_size=0.3, random_state=42)
X_train = pipeline.fit_transform(X_train)
X_test = pipeline.transform(X_test)

filename = 'training.ppl'
pickle.dump(pipeline, open(filename, 'wb'))

kernels = ['liner', 'poly']
fit_SVM(kernels, X_train, y_train, X_test, y_test, verbose=False, save=True)

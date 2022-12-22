docker login

docker build -f ./FamilyBudget/Server/Dockerfile -t psdmikolajwaluskiewicz/family-budget:latest .

docker push psdmikolajwaluskiewicz/family-budget:latest
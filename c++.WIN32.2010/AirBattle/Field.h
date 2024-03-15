#pragma once
class Field
{
public:
	Field(void);
	~Field(void);

	void setCell(int,int,int);
	int getCell(int,int);
	int shootCell(int, int);
	bool placePlane(int, int, int);

private:
	int _field[100];
	int _fieldmask[10];
};


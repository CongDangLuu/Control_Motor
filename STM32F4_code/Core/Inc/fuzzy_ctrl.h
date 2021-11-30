#ifndef FUZZY_CTRL_H_INCLUDED
#define FUZZY_CTRL_H_INCLUDED
void value_hlt_e(float*, float*, float);
void value_hlt_e_dot(float*, float*, float);
void find_beta(float*, float*, float*);
void rule_fuzzy(float*, float*, float*, float*, float*, float*);

float defuzzy_kp(float*, float*);
float defuzzy_ki(float*, float*);
float defuzzy_kd(float*, float*);

#endif // FUZZY_CTRL_H_INCLUDED

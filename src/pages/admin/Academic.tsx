import { useEffect, useState } from "react";
import Page, { AddButton } from "../../components/Page";
import {
  Empty,
  Loading,
  Modal,
  Pagination,
  SearchBox
} from "../../components/Common";
import {
  Field,
  Select,
  TextInput,
  TextArea
} from "../../components/Forms";
import { services } from "../../services";
import { apiError } from "../../lib/api";
import { useAppDispatch } from "../../hooks";
import { showToast } from "../../features/ui/uiSlice";
import type {
  ClassRoom,
  Course,
  Subject
} from "../../types";

type Kind = "classes" | "courses" | "subjects";

interface AcademicForm {
  name: string;
  code: string;
  description: string;
  classId: string;
  courseId: string;
  isActive: boolean;
}

const emptyForm = (): AcademicForm => ({
  name: "",
  code: "",
  description: "",
  classId: "",
  courseId: "",
  isActive: true
});

const meta = {
  classes: {
    title: "Classes",
    name: "class",
    endpoint: services.classes
  },
  courses: {
    title: "Courses",
    name: "course",
    endpoint: services.courses
  },
  subjects: {
    title: "Subjects",
    name: "subject",
    endpoint: services.subjects
  }
};

export default function Academic({ kind }: { kind: Kind }) {
  const m = meta[kind];

  const dispatch = useAppDispatch();

  const [items, setItems] = useState<any[]>([]);
  const [classes, setClasses] = useState<ClassRoom[]>([]);
  const [courses, setCourses] = useState<Course[]>([]);

  const [q, setQ] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  const [modal, setModal] = useState(false);
  const [editing, setEditing] = useState<any>(null);

  const [form, setForm] = useState<AcademicForm>(emptyForm());

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  // ---------------------------------------------------------
  // Load Classes / Courses required by dropdowns
  // ---------------------------------------------------------

  useEffect(() => {
    let cancelled = false;

    const loadDependencies = async () => {
      try {
        if (kind === "courses" || kind === "subjects") {
          const response = await services.classes.list({
            page: 1,
            pageSize: 100
          });

          if (!cancelled) {
            setClasses(response.items);
          }
        }

        if (kind === "subjects") {
          const response = await services.courses.list({
            page: 1,
            pageSize: 100
          });

          if (!cancelled) {
            setCourses(response.items);
          }
        }
      } catch {
        if (!cancelled) {
          dispatch(
            showToast({
              type: "error",
              message: "Failed to load academic relationships."
            })
          );
        }
      }
    };

    loadDependencies();

    return () => {
      cancelled = true;
    };
  }, [kind, dispatch]);

  // ---------------------------------------------------------
  // Load current page
  // ---------------------------------------------------------

  const load = async (p = 1) => {
    setLoading(true);

    try {
      const response = await m.endpoint.list({
        search: q.trim() || undefined,
        page: p,
        pageSize: 20
      });

      setItems(response.items);
      setTotal(response.total);
      setPage(response.page);
    } catch (error) {
      dispatch(
        showToast({
          type: "error",
          message: apiError(error)
        })
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [q, kind]);

  // ---------------------------------------------------------
  // Open create/edit modal
  // ---------------------------------------------------------

  const open = (item?: any) => {
    if (item) {
      setEditing(item);

      setForm({
        name: item.name ?? "",
        code: item.code ?? "",
        description: item.description ?? "",
        classId: item.classId ?? "",
        courseId: item.courseId ?? "",
        isActive: item.isActive ?? true
      });
    } else {
      setEditing(null);
      setForm(emptyForm());
    }

    setModal(true);
  };

  // ---------------------------------------------------------
  // Validation
  // ---------------------------------------------------------

  const validate = () => {
    if (!form.name.trim()) {
      dispatch(
        showToast({
          type: "error",
          message: "Name is required."
        })
      );

      return false;
    }

    if (!form.code.trim()) {
      dispatch(
        showToast({
          type: "error",
          message: "Code is required."
        })
      );

      return false;
    }

    if (kind === "courses" && !form.classId) {
      dispatch(
        showToast({
          type: "error",
          message: "Please select a class."
        })
      );

      return false;
    }

    if (kind === "subjects" && !form.courseId) {
      dispatch(
        showToast({
          type: "error",
          message: "Please select a course."
        })
      );

      return false;
    }

    return true;
  };

  // ---------------------------------------------------------
  // Save
  // ---------------------------------------------------------

  const save = async () => {
    if (!validate()) {
      return;
    }

    setSaving(true);

    try {
      const payload =
        kind === "classes"
          ? {
              name: form.name.trim(),
              code: form.code.trim(),
              description: form.description.trim()
            }
          : kind === "courses"
          ? {
              name: form.name.trim(),
              code: form.code.trim(),
              description: form.description.trim(),
              classId: form.classId
            }
          : {
              name: form.name.trim(),
              code: form.code.trim(),
              description: form.description.trim(),
              courseId: form.courseId
            };

      if (editing) {
        await m.endpoint.update(editing.id, {
          ...payload,
          isActive: form.isActive
        });
      } else {
        await m.endpoint.create(payload);
      }

      setModal(false);

      dispatch(
        showToast({
          type: "success",
          message: `${m.name} ${
            editing ? "updated" : "created"
          } successfully.`
        })
      );

      await load(page);
    } catch (error) {
      dispatch(
        showToast({
          type: "error",
          message: apiError(error)
        })
      );
    } finally {
      setSaving(false);
    }
  };

  // ---------------------------------------------------------
  // Delete
  // ---------------------------------------------------------

  const remove = async (id: string) => {
    if (!window.confirm(`Delete this ${m.name}?`)) {
      return;
    }

    try {
      await m.endpoint.remove(id);

      dispatch(
        showToast({
          type: "success",
          message: `${m.name} deleted successfully.`
        })
      );

      await load(page);
    } catch (error) {
      dispatch(
        showToast({
          type: "error",
          message: apiError(error)
        })
      );
    }
  };

  return (
    <Page
      title={m.title}
      subtitle={`Manage ${m.title.toLowerCase()} and their relationships.`}
      action={
        <AddButton onClick={() => open()}>
          Add {m.name}
        </AddButton>
      }
    >
      <div className="toolbar">
        <SearchBox
          value={q}
          onChange={setQ}
        />
      </div>

      <div className="table-card">
        {loading ? (
          <Loading />
        ) : items.length === 0 ? (
          <Empty />
        ) : (
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Code</th>

                {kind === "courses" && (
                  <th>Class</th>
                )}

                {kind === "subjects" && (
                  <th>Course</th>
                )}

                <th>Status</th>
                <th />
              </tr>
            </thead>

            <tbody>
              {items.map((item) => (
                <tr key={item.id}>
                  <td>
                    <b>{item.name}</b>

                    <small>
                      {item.description}
                    </small>
                  </td>

                  <td>{item.code}</td>

                  {kind === "courses" && (
                    <td>
                      {
                        classes.find(
                          (x) => x.id === item.classId
                        )?.name
                      }

                      {!classes.find(
                        (x) => x.id === item.classId
                      ) && item.classId}
                    </td>
                  )}

                  {kind === "subjects" && (
                    <td>
                      {
                        courses.find(
                          (x) => x.id === item.courseId
                        )?.name
                      }

                      {!courses.find(
                        (x) => x.id === item.courseId
                      ) && item.courseId}
                    </td>
                  )}

                  <td>
                    {item.isActive
                      ? "Active"
                      : "Inactive"}
                  </td>

                  <td>
                    <div className="row-actions">
                      <button
                        className="btn btn-sm btn-secondary"
                        onClick={() => open(item)}
                      >
                        Edit
                      </button>

                      <button
                        className="btn btn-sm btn-danger"
                        onClick={() => remove(item.id)}
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Pagination
        page={page}
        pageSize={20}
        total={total}
        onPage={load}
      />

      <Modal
        open={modal}
        onClose={() => !saving && setModal(false)}
        title={`${editing ? "Edit" : "Create"} ${m.name}`}
      >
        <Field label="Name">
          <TextInput
            value={form.name}
            onChange={(e) =>
              setForm({
                ...form,
                name: e.target.value
              })
            }
          />
        </Field>

        <Field label="Code">
          <TextInput
            value={form.code}
            onChange={(e) =>
              setForm({
                ...form,
                code: e.target.value
              })
            }
          />
        </Field>

        <Field label="Description">
          <TextArea
            value={form.description}
            onChange={(e) =>
              setForm({
                ...form,
                description: e.target.value
              })
            }
          />
        </Field>

        {kind === "courses" && (
          <Field label="Class">
            <Select
              value={form.classId}
              onChange={(e) =>
                setForm({
                  ...form,
                  classId: e.target.value
                })
              }
            >
              <option value="">
                Select Class...
              </option>

              {classes.map((item) => (
                <option
                  key={item.id}
                  value={item.id}
                >
                  {item.name} ({item.code})
                </option>
              ))}
            </Select>
          </Field>
        )}

        {kind === "subjects" && (
          <Field label="Course">
            <Select
              value={form.courseId}
              onChange={(e) =>
                setForm({
                  ...form,
                  courseId: e.target.value
                })
              }
            >
              <option value="">
                Select Course...
              </option>

              {courses.map((item) => (
                <option
                  key={item.id}
                  value={item.id}
                >
                  {item.name} ({item.code})
                </option>
              ))}
            </Select>
          </Field>
        )}

        {editing && (
          <label className="check">
            <input
              type="checkbox"
              checked={form.isActive}
              onChange={(e) =>
                setForm({
                  ...form,
                  isActive: e.target.checked
                })
              }
            />

            Active
          </label>
        )}

        <div className="modal-actions">
          <button
            className="btn btn-secondary"
            onClick={() => setModal(false)}
            disabled={saving}
          >
            Cancel
          </button>

          <button
            className="btn btn-primary"
            onClick={save}
            disabled={saving}
          >
            {saving
              ? "Saving..."
              : editing
              ? "Update"
              : "Create"}
          </button>
        </div>
      </Modal>
    </Page>
  );
}